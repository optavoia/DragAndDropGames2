using UnityEngine;
using UnityEngine.EventSystems;

public class DiscUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PegUI currentPeg;          // назначай в инспекторе или через инициализатор
    private RectTransform rect;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        // если currentPeg задан, зарегистрируемся в нём
        if (currentPeg != null)
        {
            currentPeg.AddDisc(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Можно брать только верхний диск столба
        if (currentPeg == null || currentPeg.TopDisc() != this) return;

        canvasGroup.blocksRaycasts = false; // чтобы FindNearestPeg мог отловить пеги под курсором
        // опционально: можно уменьшить alpha -> canvasGroup.alpha = 0.8f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentPeg == null) return;
        // передвигаем в UI-координатах с учётом scaleFactor
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentPeg == null) return;
        canvasGroup.blocksRaycasts = true;
        PegUI nearest = FindNearestPeg();
        if (nearest != null && CanPlace(nearest))
        {
            // убрать с текущего пега
            currentPeg.RemoveDisc();
            currentPeg = nearest;

            // Важно: делаем диск дочерним нового пега
            rect.SetParent(nearest.GetComponent<RectTransform>(), false);

            nearest.AddDisc(this);
        }
        else
        {
            currentPeg.PositionDiscs(); // вернуть на своё место
        }

    }

    private PegUI FindNearestPeg()
    {
        PegUI[] pegs = FindObjectsOfType<PegUI>();
        if (pegs == null || pegs.Length == 0) return null;

        PegUI best = pegs[0];
        float bestDist = Vector2.Distance(rect.position, pegs[0].transform.position);

        foreach (PegUI p in pegs)
        {
            float d = Vector2.Distance(rect.position, p.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = p;
            }
        }
        return best;
    }

    private bool CanPlace(PegUI peg)
    {
        DiscUI top = peg.TopDisc();
        if (top == null) return true; // пустой столб — можно ставить

        float myWidth = rect.sizeDelta.x;
        float topWidth = top.GetComponent<RectTransform>().sizeDelta.x;
        return myWidth < topWidth; // можно ставить, только если ширина меньше
    }
}

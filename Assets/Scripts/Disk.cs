using UnityEngine;
using UnityEngine.EventSystems;

public class Disk : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int size; // 1 small, 2 medium, 3 large

    private RectTransform rect;
    private CanvasGroup cg;

    private Vector3 startPos;
    private Transform startParent;
    private PegUI startPeg;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = rect.position;
        startParent = transform.parent;

        // Правильно ищем PegUI
        startPeg = startParent.GetComponentInParent<PegUI>();

        if (startPeg.PeekDisk() != this)
        {
            eventData.pointerDrag = null;
            return;
        }

        cg.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cg.blocksRaycasts = true;

        PegUI targetPeg = DragTargetPegUI.targetPeg;

        if (targetPeg == null)
        {
            ReturnToStart();
            return;
        }

        Disk top = targetPeg.PeekDisk();

        if (top != null && top.size < this.size)
        {
            ReturnToStart();
            return;
        }

        startPeg.PopDisk();
        targetPeg.PushDisk(this);

        FindObjectOfType<GameManagerUI>().RegisterMove();

        DragTargetPegUI.targetPeg = null;
    }

    void ReturnToStart()
    {
        rect.position = startPos;
        transform.SetParent(startParent);

        // Звук ошибки
        var gm = FindObjectOfType<GameManagerUI>();
        if (gm != null && gm.audioSource != null && gm.errorSound != null)
            gm.audioSource.PlayOneShot(gm.errorSound);
    }
}

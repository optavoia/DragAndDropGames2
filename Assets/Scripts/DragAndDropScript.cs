using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DragAndDropScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, 
    IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGro;
    private RectTransform rectTra;
    public ObjectScript objectScr;
    public ScreenBoundriesScript screenBou;
    private DraggableItem draggableItem; // ссылка на скрипт с locked


    // Start is called before the first frame update
    void Start()
    {
        canvasGro = GetComponent<CanvasGroup>();
        rectTra = GetComponent<RectTransform>();
        draggableItem = GetComponent<DraggableItem>(); // получаем компонент DraggableItem
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (draggableItem != null && draggableItem.locked)
            return; // если объект заблокирован, игнорируем клик

        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            Debug.Log("OnPointerDown");
            objectScr.effects.PlayOneShot(objectScr.audioCli[0]);
        } 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (draggableItem != null && draggableItem.locked)
            return; // нельзя начинать перетаскивание
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            ObjectScript.drag = true;
            ObjectScript.lastDragged = eventData.pointerDrag;
            canvasGro.blocksRaycasts = false;
            canvasGro.alpha = 0.6f;
            //rectTra.SetAsLastSibling();
            // Находим индекс BlockerPanel, чтобы не подниматься выше неё
            Transform blocker = transform.parent.Find("BlockerPanel");
            if (blocker != null)
            {
                int blockerIndex = blocker.GetSiblingIndex();
                transform.SetSiblingIndex(Mathf.Max(0, blockerIndex - 1));
            }
            else
            {
                transform.SetAsLastSibling(); // fallback, если панель не найдена
            }
            Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(
               new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenBou.screenPoint.z));
            rectTra.position = cursorWorldPos;

            screenBou.screenPoint = Camera.main.WorldToScreenPoint(rectTra.localPosition);

            screenBou.offset = rectTra.localPosition - 
                Camera.main.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
                screenBou.screenPoint.z));
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggableItem != null && draggableItem.locked)
            return; // игнорируем движение
        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            Vector3 curSreenPoint = 
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenBou.screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curSreenPoint) + screenBou.offset;
            rectTra.position = screenBou.GetClampedPosition(curPosition);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGro.alpha = 1f;
        if (draggableItem != null && draggableItem.locked)
            return; // игнорируем отпускание

        if (Input.GetMouseButtonUp(0))
        {
            ObjectScript.drag = false;
            canvasGro.blocksRaycasts = true;
            canvasGro.alpha = 1f;

            if(objectScr.rightPlace)
            {
                
                ObjectScript.lastDragged = null;
                canvasGro.blocksRaycasts = true; // включаем блокировку кликов, чтобы его не трогали

                // 🔒 блокируем объект, если он встал на правильное место
                if (draggableItem != null)
                    draggableItem.locked = true;


            }
            
            objectScr.rightPlace = false;
        }
    }
}

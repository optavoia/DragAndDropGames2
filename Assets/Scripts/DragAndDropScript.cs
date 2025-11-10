using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// ANDROID VERSION
public class DragAndDropScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler,
    IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGro;
    private RectTransform rectTra;
    public ObjectScript objectScr;
    public ScreenBoundriesScript screenBou;
    private DraggableItem draggableItem; // ссылка на скрипт с locked

    private Vector3 dragOffsetWorld;
    private Camera uiCamera;
    private Canvas canvas;


    void Awake()
    {
        canvasGro = GetComponent<CanvasGroup>();
        rectTra = GetComponent<RectTransform>();
        draggableItem = GetComponent<DraggableItem>();

        if (objectScr == null)
            objectScr = Object.FindFirstObjectByType<ObjectScript>();

        if (screenBou == null)
            screenBou = Object.FindFirstObjectByType<ScreenBoundriesScript>();

        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            uiCamera = canvas.worldCamera;
        }
        else
        {
            Debug.LogError("Canvas not found for DragAndDropScript");
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (draggableItem != null && draggableItem.locked)
            return; // если объект заблокирован, игнорируем клик

        Debug.Log("OnPointerDown");
        objectScr.effects.PlayOneShot(objectScr.audioCli[0]);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (draggableItem != null && draggableItem.locked)
            return; // нельзя начинать перетаскивание

        ObjectScript.drag = true;
        ObjectScript.lastDragged = eventData.pointerDrag;
        canvasGro.blocksRaycasts = false;
        canvasGro.alpha = 0.6f;

        // чтобы не перекрывать BlockerPanel
        Transform blocker = transform.parent.Find("BlockerPanel");
        if (blocker != null)
        {
            int blockerIndex = blocker.GetSiblingIndex();
            transform.SetSiblingIndex(Mathf.Max(0, blockerIndex - 1));
        }
        else
        {
            transform.SetAsLastSibling();
        }

        if (ScreenPointToWorld(eventData.position, out Vector3 pointerWorld))
        {
            dragOffsetWorld = transform.position - pointerWorld;
        }
        else
        {
            dragOffsetWorld = Vector3.zero;
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (draggableItem != null && draggableItem.locked)
            return; // игнорируем движение, если заблокирован

        if (!ScreenPointToWorld(eventData.position, out Vector3 pointerWorld))
            return;

        Vector3 desiredPosition = pointerWorld + dragOffsetWorld;
        desiredPosition.z = transform.position.z;

        screenBou.RecalculateBounds();
        Vector2 clamped = screenBou.GetClampedPosition(desiredPosition);
        transform.position = new Vector3(clamped.x, clamped.y, desiredPosition.z);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGro.alpha = 1f;

        if (draggableItem != null && draggableItem.locked)
            return; // игнорируем отпускание

        ObjectScript.drag = false;
        
        canvasGro.blocksRaycasts = true;
        canvasGro.alpha = 1.0f;

        if (objectScr.rightPlace)
        {
            // объект на правильном месте
            ObjectScript.lastDragged = null;
            canvasGro.blocksRaycasts = true; 

            // блокируем объект, если он встал на своё место
            if (draggableItem != null)
                draggableItem.locked = true;
        }

        // Сбрасываем флаг
        objectScr.rightPlace = false;
    }


    private bool ScreenPointToWorld(Vector2 screenPoint, out Vector3 worldPoint)
    {
        worldPoint = Vector3.zero;

        if (uiCamera == null)
            return false;

        float z = Mathf.Abs(uiCamera.transform.position.z - transform.position.z);
        Vector3 sp = new Vector3(screenPoint.x, screenPoint.y, z);
        worldPoint = uiCamera.ScreenToWorldPoint(sp);

        return true;
    }
}

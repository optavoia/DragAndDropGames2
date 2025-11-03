using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    private float placeZRot, vehicleZRot, rotDiff;
    private Vector3 placeSiz, vehicleSiz;
    private float xSizeDiff, ySizeDiff;
    public ObjectScript objScript;

    void Start()
    {
        if (objScript == null)
        {
            objScript = Object.FindFirstObjectByType<ObjectScript>();
        }
    }

    // Этот метод вызывается при отпускании перетаскиваемого объекта над зоной Drop
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        // ✅ Универсальная проверка "отпускания" для ПК и Android
        bool released = false;

#if UNITY_EDITOR || UNITY_STANDALONE
        // ПК / Editor
        released = Input.GetMouseButtonUp(0);
#elif UNITY_ANDROID || UNITY_IOS
        // Мобильные устройства
        released = Input.touchCount == 0;
#endif

        if (!released)
            return;

        // Проверка совпадения по тегу (объект и место должны совпадать)
        if (eventData.pointerDrag.tag.Equals(tag))
        {
            // Проверяем поворот
            placeZRot = eventData.pointerDrag.GetComponent<RectTransform>().transform.eulerAngles.z;
            vehicleZRot = GetComponent<RectTransform>().transform.eulerAngles.z;
            rotDiff = Mathf.Abs(placeZRot - vehicleZRot);
            Debug.Log("Rotation difference: " + rotDiff);

            // Проверяем размер
            placeSiz = eventData.pointerDrag.GetComponent<RectTransform>().localScale;
            vehicleSiz = GetComponent<RectTransform>().localScale;
            xSizeDiff = Mathf.Abs(placeSiz.x - vehicleSiz.x);
            ySizeDiff = Mathf.Abs(placeSiz.y - vehicleSiz.y);
            Debug.Log("X size difference: " + xSizeDiff);
            Debug.Log("Y size difference: " + ySizeDiff);

            // Если объект подходит по размеру и вращению
            if ((rotDiff <= 5 || (rotDiff >= 355 && rotDiff <= 360)) &&
                (xSizeDiff <= 0.05 && ySizeDiff <= 0.05))
            {
                Debug.Log("Correct place");

                // Совмещаем объект с местом
                var rect = eventData.pointerDrag.GetComponent<RectTransform>();
                var targetRect = GetComponent<RectTransform>();
                rect.localPosition = targetRect.localPosition;
                rect.localRotation = targetRect.localRotation;
                rect.localScale = targetRect.localScale;

                // 🔒 Блокируем объект, чтобы его нельзя было снова переместить
                var draggable = eventData.pointerDrag.GetComponent<DraggableItem>();
                if (draggable != null)
                    draggable.locked = true;

                // Сообщаем менеджеру, что объект успешно размещён
                GameManager.Instance.CarPlaced();

                // 🔊 Звук по тегу
                switch (eventData.pointerDrag.tag)
                {
                    case "Garbage": objScript.effects.PlayOneShot(objScript.audioCli[1]); break;
                    case "Medicine": objScript.effects.PlayOneShot(objScript.audioCli[2]); break;
                    case "Fire": objScript.effects.PlayOneShot(objScript.audioCli[3]); break;
                    case "Buss": objScript.effects.PlayOneShot(objScript.audioCli[4]); break;
                    case "b2": objScript.effects.PlayOneShot(objScript.audioCli[5]); break;
                    case "cement": objScript.effects.PlayOneShot(objScript.audioCli[6]); break;
                    case "eskavator": objScript.effects.PlayOneShot(objScript.audioCli[7]); break;
                    case "policija": objScript.effects.PlayOneShot(objScript.audioCli[8]); break;
                    case "Tracktor": objScript.effects.PlayOneShot(objScript.audioCli[9]); break;
                    case "masina": objScript.effects.PlayOneShot(objScript.audioCli[10]); break;
                    default: Debug.Log("Unknown tag detected"); break;
                }
            }
        }
        else
        {
            // ❌ Объект помещён не в то место
            objScript.rightPlace = false;
            objScript.effects.PlayOneShot(objScript.audioCli[0]);

            // Возвращаем объект на стартовую позицию
            for (int i = 0; i < objScript.vehicles.Length; i++)
            {
                if (objScript.vehicles[i].CompareTag(eventData.pointerDrag.tag))
                {
                    objScript.vehicles[i].GetComponent<RectTransform>().localPosition =
                        objScript.startCoordinates[i];
                    break;
                }
            }
        }
    }
}

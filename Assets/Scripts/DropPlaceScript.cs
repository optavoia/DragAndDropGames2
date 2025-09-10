using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript:  MonoBehaviour, IDropHandler
{

    private float placeZRot, vehicleZRot, rotDiff;
    private Vector3 placeSiz, vehicleSiz;
    private float xSizeDiff, ySizeDiff;
    public ObjectScript objectScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnDrop(PointerEventData eventData)
    {
        if((eventData.pointerDrag != null) && Input.GetMouseButtonUp(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            if (eventData.pointerDrag.tag.Equals(tag))
            {
                placeZRot = eventData.pointerDrag.GetComponent<RectTransform>().transform.eulerAngles.z;

                vehicleZRot = GetComponent<RectTransform>().transform.eulerAngles.z;

                rotDiff = Mathf.Abs(placeZRot - vehicleZRot);
                Debug.Log("ROtation difference: " + rotDiff);

                placeSiz = eventData.pointerDrag.GetComponent<RectTransform>().localScale;
                vehicleSiz = GetComponent<RectTransform>().localScale;
                xSizeDiff = Mathf.Abs(placeSiz.x - vehicleSiz.x);
                ySizeDiff = Mathf.Abs(placeSiz.y - vehicleSiz.y);
                Debug.Log("X size difference: " + xSizeDiff);
                Debug.Log("Y size difference: " + ySizeDiff);

                if((rotDiff <= 5) || (rotDiff >= 355 && rotDiff <= 360) &&
                    (xSizeDiff <= 0.05 && ySizeDiff <= 0.05)){
                    Debug.Log("COrrect place");
                }
            }
            else
            {
                objectScript.rightPlace = false;
                objectScript.effects.PlayOneShot(objectScript.audioCli[1]);

                switch (eventData.pointerDrag.tag)
                {
                    case "Garbage":
                        objectScript.vehicles[0].GetComponent<RectTransform>().localPosition = 
                            objectScript.startCoor[0];
                        break;

                    case "Medicine":
                        objectScript.vehicles[1].GetComponent<RectTransform>().localPosition =
                            objectScript.startCoor[1];
                        break;
                    case "Fire":
                        objectScript.vehicles[2].GetComponent<RectTransform>().localPosition =
                            objectScript.startCoor[2];
                        break;
                    case "Buss":
                        objectScript.vehicles[3].GetComponent<RectTransform>().localPosition =
                            objectScript.startCoor[3];
                        break;
                    case "b2":
                        objectScript.vehicles[4].GetComponent<RectTransform>().localPosition =
                            objectScript.startCoor[4];
                        break;
                    case "cement":
                        objectScript.vehicles[5].GetComponent<RectTransform>().localPosition =
                            objectScript.startCoor[5];
                        break;
                    default:
                        Debug.Log("Unknown tag detected");
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

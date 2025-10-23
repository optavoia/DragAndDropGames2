﻿using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlaceScript : MonoBehaviour, IDropHandler
{
    private float placeZRot, vehicleZRot, rotDiff;
    private Vector3 placeSiz, vehicleSiz;
    private float xSizeDiff, ySizeDiff;
    public ObjectScript objScript;

    void Start()
    {
        if(objScript == null)
        {
            objScript = Object.FindFirstObjectByType<ObjectScript>();
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        if (eventData.pointerDrag.tag.Equals(tag))
        {
            placeZRot = eventData.pointerDrag.GetComponent<RectTransform>().transform.eulerAngles.z;
        }
        if((eventData.pointerDrag != null) && 
            Input.GetMouseButtonUp(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            if(eventData.pointerDrag.tag.Equals(tag))
            {
               placeZRot = 
                    eventData.pointerDrag.GetComponent<RectTransform>().transform.eulerAngles.z;
                
                vehicleZRot = 
                    GetComponent<RectTransform>().transform.eulerAngles.z;

                rotDiff = Mathf.Abs(placeZRot - vehicleZRot);
                Debug.Log("Rotation difference: " + rotDiff);

                placeSiz = eventData.pointerDrag.GetComponent<RectTransform>().localScale;
                vehicleSiz = GetComponent<RectTransform>().localScale;
                xSizeDiff = Mathf.Abs(placeSiz.x - vehicleSiz.x);
                ySizeDiff = Mathf.Abs(placeSiz.y - vehicleSiz.y);
                Debug.Log("X size difference: " + xSizeDiff);
                Debug.Log("Y size difference: " + ySizeDiff);

                if((rotDiff <= 5 || (rotDiff >= 355 && rotDiff <= 360)) &&
                    (xSizeDiff <= 0.05 && ySizeDiff <= 0.05)) {
                    Debug.Log("Correct place");

                    eventData.pointerDrag.GetComponent<RectTransform>().localPosition =
                        GetComponent<RectTransform>().localPosition;
                    eventData.pointerDrag.GetComponent<RectTransform>().localRotation =
                        GetComponent<RectTransform>().localRotation;
                    eventData.pointerDrag.GetComponent<RectTransform>().localScale =
                        GetComponent<RectTransform>().localScale ;

                    // 🔒 Блокируем объект, чтобы его больше нельзя было двигать
                    var draggable = eventData.pointerDrag.GetComponent<DraggableItem>();
                    if (draggable != null)
                        draggable.locked = true;

                    GameManager.Instance.CarPlaced();

                    switch (eventData.pointerDrag.tag)
                    {
                        case "Garbage":
                            objScript.effects.PlayOneShot(objScript.audioCli[1]);
                            break;
                        case "Medicine":
                            objScript.effects.PlayOneShot(objScript.audioCli[2]);
                            break;
                        case "Fire":
                            objScript.effects.PlayOneShot(objScript.audioCli[3]);
                            break;
                        case "Buss":
                            objScript.effects.PlayOneShot(objScript.audioCli[4]);
                            break;
                        case "b2":
                            objScript.effects.PlayOneShot(objScript.audioCli[5]);
                            break;
                        case "cement":
                            objScript.effects.PlayOneShot(objScript.audioCli[6]);
                            break;
                        case "eskavator":
                            objScript.effects.PlayOneShot(objScript.audioCli[7]);
                            break;
                        case "policija":
                            objScript.effects.PlayOneShot(objScript.audioCli[8]);
                            break;
                        case "Tracktor":
                            objScript.effects.PlayOneShot(objScript.audioCli[9]);
                            break;
                        case "masina":
                            objScript.effects.PlayOneShot(objScript.audioCli[10]);
                            break;
                        default:
                            Debug.Log("UNknown tag detekted");
                            break;
                    }
                }

            } else
            {
                objScript.rightPlace = false;
                objScript.effects.PlayOneShot(objScript.audioCli[0]);

                switch(eventData.pointerDrag.tag)
                {
                    case "Garbage":
                        objScript.vehicles[0].GetComponent<RectTransform>().localPosition = 
                            objScript.startCoordinates[0];
                        break;

                    case "Medicine":
                        objScript.vehicles[1].GetComponent<RectTransform>().localPosition =
                           objScript.startCoordinates[1];
                        break;

                    case "Fire":
                        objScript.vehicles[2].GetComponent<RectTransform>().localPosition =
                           objScript.startCoordinates[2];
                        break;
                    case "Buss":
                        objScript.vehicles[3].GetComponent<RectTransform>().localPosition =
                           objScript.startCoordinates[3];
                        break;
                    case "b2":
                        objScript.vehicles[4].GetComponent<RectTransform>().localPosition =
                           objScript.startCoordinates[4];
                        break;
                    case "cement":
                        objScript.vehicles[5].GetComponent<RectTransform>().localPosition =
                           objScript.startCoordinates[5];
                        break;
                    case "eskavator":
                        objScript.vehicles[6].GetComponent<RectTransform>().localPosition =
                           objScript.startCoordinates[6];
                        break;
                    case "policija":
                        objScript.vehicles[7].GetComponent<RectTransform>().localPosition =
                           objScript.startCoordinates[7];
                        break;
                    case "Tracktor":
                        objScript.vehicles[8].GetComponent<RectTransform>().localPosition =
                           objScript.startCoordinates[8];
                        break;
                    case "masina":
                        objScript.vehicles[9].GetComponent<RectTransform>().localPosition =
                           objScript.startCoordinates[9];
                        break;
                    default:
                        Debug.Log("Unknown tag detected");
                        break;
                }
            }
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class DragTargetPegUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static PegUI targetPeg;
    public PegUI thisPeg;

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetPeg = thisPeg;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetPeg == thisPeg)
            targetPeg = null;
    }
}

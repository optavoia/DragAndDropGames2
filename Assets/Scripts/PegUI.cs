using System;
using System.Collections.Generic;
using UnityEngine;

public class PegUI : MonoBehaviour
{
    // стек дисков (тип DiscUI)
    public Stack<DiscUI> discs = new Stack<DiscUI>();

    public DiscUI TopDisc()
    {
        return discs.Count == 0 ? null : discs.Peek();
    }

    public void AddDisc(DiscUI d)
    {
        if (d == null) return;
        discs.Push(d);
        PositionDiscs();
    }

    public DiscUI RemoveDisc()
    {
        if (discs.Count == 0) return null;
        DiscUI removed = discs.Pop();
        PositionDiscs();
        return removed;
    }

    public void PositionDiscs()
    {
        float yStep = 60f; // расстояние между дисками
        DiscUI[] arr = discs.ToArray();
        Array.Reverse(arr); // чтобы нижний диск был первым

        RectTransform pegRT = GetComponent<RectTransform>();

        for (int i = 0; i < arr.Length; i++)
        {
            RectTransform rt = arr[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(
                0, // относительно столба по X
                i * yStep // снизу вверх по Y
            );
            rt.localRotation = Quaternion.identity; // сброс поворота
            rt.SetParent(pegRT, false); // диски должны быть детьми пега
        }
    }

}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameInitializerUI : MonoBehaviour
{
    public PegUI pegA;
    public PegUI pegB;
    public PegUI pegC;
    public List<DiscUI> allDiscs; // перетащи сюда все диски (или оставь пустым, тогда будет FindObjectsOfType)

    void Start()
    {
        if (allDiscs == null || allDiscs.Count == 0)
        {
            allDiscs = FindObjectsOfType<DiscUI>().ToList();
        }

        // сортируем по ширине (sizeDelta.x) — от большого к маленькому
        allDiscs.Sort((d1, d2) => d2.GetComponent<RectTransform>().sizeDelta.x
                                    .CompareTo(d1.GetComponent<RectTransform>().sizeDelta.x));

        // очищаем пеги (если что-то было)
        pegA.discs.Clear();
        pegB.discs.Clear();
        pegC.discs.Clear();

        // размещаем все диски на pegA: первый — самый большой (вниз)
        foreach (DiscUI d in allDiscs)
        {
            d.currentPeg = pegA;
            pegA.AddDisc(d);
        }
    }
}

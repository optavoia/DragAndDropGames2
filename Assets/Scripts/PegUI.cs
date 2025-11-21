using System.Collections.Generic;
using UnityEngine;

public class PegUI : MonoBehaviour
{
    public RectTransform diskBase;
    public List<Disk> disks = new List<Disk>();

    public void PushDisk(Disk disk)
    {
        disks.Add(disk);

        disk.transform.SetParent(diskBase);
        AnimateDiskToPosition(disk, disks.Count - 1);
    }

    public Disk PopDisk()
    {
        Disk d = disks[disks.Count - 1];
        disks.RemoveAt(disks.Count - 1);
        return d;
    }

    public Disk PeekDisk()
    {
        if (disks.Count == 0) return null;
        return disks[disks.Count - 1];
    }

    public void AnimateDiskToPosition(Disk disk, int index)
    {
        Vector2 target = new Vector2(0, index * 60);
        disk.StartCoroutine(SmoothMove(disk.GetComponent<RectTransform>(), target));
    }

    private System.Collections.IEnumerator SmoothMove(RectTransform rect, Vector2 target)
    {
        Vector2 start = rect.anchoredPosition;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 4f;
            rect.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        rect.anchoredPosition = target;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    internal InterstitialAd interactable;

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => AudioManager.Instance.PlayClick());
        }
    }
}

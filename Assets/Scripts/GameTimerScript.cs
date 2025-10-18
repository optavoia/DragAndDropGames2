using TMPro;
using UnityEngine;

public class GameTimerScript : MonoBehaviour
{
    public TMP_Text timerText;
    private float elapsedTime = 0f;

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.gameActive)
            return;

        elapsedTime += Time.deltaTime;

        int hours = Mathf.FloorToInt(elapsedTime / 3600f);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
    public void ResetTimer()
    {
        elapsedTime = 0f;
        timerText.text = "00:00:00";
    }
}

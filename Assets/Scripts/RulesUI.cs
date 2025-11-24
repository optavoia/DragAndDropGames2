using UnityEngine;

public class RulesUI : MonoBehaviour
{
    public GameObject rulesPanel;   // панель правил

    public void ShowRules()
    {
        rulesPanel.SetActive(true);
        Time.timeScale = 0f; // пауза игры
    }

    public void HideRules()
    {
        rulesPanel.SetActive(false);
        Time.timeScale = 1f; // продолжаем игру
    }
}
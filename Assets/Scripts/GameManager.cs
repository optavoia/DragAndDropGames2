using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Stats")]
    public int totalCars;      // всего машин
    public int placedCars;     // сколько поставлено
    public int eatenCars;      // сколько съедено
    public float timer = 0f;
    public bool gameActive = true;

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject blockerPanel;

    [Header("UI Texts")]
    public TMP_Text gameTimerText;     // основной таймер на экране во время игры
    public TMP_Text winTimerText;      // таймер на панели победы
    public TMP_Text loseTimerText;     // таймер на панели поражения
    public TMP_Text winScoreText;      // оценка на панели победы
    public TMP_Text loseScoreText;     // оценка на панели поражения


    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (gameActive)
        {
            timer += Time.deltaTime;
            int hours = (int)(timer / 3600);
            int minutes = (int)((timer % 3600) / 60);
            int seconds = (int)(timer % 60);
            if (gameTimerText != null)
                gameTimerText.text = $"{hours:00}:{minutes:00}:{seconds:00}";
        }
    }

    public void CarPlaced()
    {
        placedCars++;
        CheckEndGame();
    }

    public void CarEaten()
    {
        eatenCars++;
        CheckEndGame();
    }

    private void CheckEndGame()
    {
        // если все машины съедены и ни одна не поставлена
        if (eatenCars >= totalCars && placedCars == 0)
        {
            Lose();
        }
        // если все машины либо поставлены, либо съедены
        else if ((eatenCars + placedCars) >= totalCars)
        {
            Win();
        }
    }

    private void Win()
    {
        gameActive = false;
        winPanel.SetActive(true);
        blockerPanel.SetActive(true);  // Блокируем клики сзади

        int hours = (int)(timer / 3600);
        int minutes = (int)((timer % 3600) / 60);
        int seconds = (int)(timer % 60);

        if (gameTimerText != null)
            gameTimerText.text = $"{hours:00}:{minutes:00}:{seconds:00} (stop)";
        if (winTimerText != null)
            winTimerText.text = $"{hours:00}:{minutes:00}:{seconds:00}";

        winScoreText.text = $"Novērtējums: {CalculateScore()}/3";
    }

    private void Lose()
    {
        gameActive = false;
        losePanel.SetActive(true);
        blockerPanel.SetActive(true);  // Блокируем клики сзади

        int hours = (int)(timer / 3600);
        int minutes = (int)((timer % 3600) / 60);
        int seconds = (int)(timer % 60);

        if (gameTimerText != null)
            gameTimerText.text = $"{hours:00}:{minutes:00}:{seconds:00} (stop)";
        if (loseTimerText != null)
            loseTimerText.text = $"{hours:00}:{minutes:00}:{seconds:00}";

        loseScoreText.text = $"{CalculateScore()}/3";
    }

    private int CalculateScore()
    {
        // Расчёт оценки в зависимости от количества поставленных машин и скорости
        float efficiency = (float)placedCars / totalCars;
        int scoreValue = 1;

        if (efficiency >= 0.66f)
            scoreValue = 3;
        else if (efficiency >= 0.33f)
            scoreValue = 2;

        // Можно добавить бонус за быстрое прохождение
        if (timer < 60f && scoreValue > 1)
            scoreValue = Mathf.Min(3, scoreValue + 1);

        return Mathf.Clamp(scoreValue, 1, 3);
    }
}

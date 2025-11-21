using UnityEngine;
using UnityEngine.UI;

public class GameManagerUI : MonoBehaviour
{
    [Header("Звуки")]
    public AudioSource audioSource;
    public AudioClip errorSound;   // звук неправильного хода

    [Header("3 ПРЕФАБА ДИСКОВ (ТВОИ КАРТИНКИ)")]
    public Disk bigDiskPrefab;     // size = 3
    public Disk mediumDiskPrefab;  // size = 2
    public Disk smallDiskPrefab;   // size = 1

    [Header("UI внутри панели победы")]
    public Text winMovesText;

    [Header("3 СТОЛБА")]
    public PegUI peg1;
    public PegUI peg2;
    public PegUI peg3;
    

    [Header("UI")]
    public Text movesText;
    public GameObject winPanel;

    private int moves = 0;

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        moves = 0;
        UpdateMoves();

        if (winPanel != null)
            winPanel.SetActive(false);

        ClearPeg(peg1);
        ClearPeg(peg2);
        ClearPeg(peg3);

        // ВАЖНО: сначала большой, потом средний, потом маленький
        SpawnDisk(bigDiskPrefab, peg1);      
        SpawnDisk(mediumDiskPrefab, peg1);  
        SpawnDisk(smallDiskPrefab, peg1);   
    }

    private void ClearPeg(PegUI peg)
    {
        foreach (var d in peg.disks)
            Destroy(d.gameObject);

        peg.disks.Clear();
    }

    private void SpawnDisk(Disk prefab, PegUI peg)
    {
        Disk d = Instantiate(prefab);
        d.transform.SetParent(peg.diskBase);
        d.transform.localScale = Vector3.one;

        peg.PushDisk(d);
    }

    public void RegisterMove()
    {
        moves++;
        UpdateMoves();
        CheckWin();
    }

    private void UpdateMoves()
    {
        if (movesText != null)
            movesText.text = "Moves: " + moves;
    }

    private void BlockAllDisks()
    {
        Disk[] allDisks = FindObjectsOfType<Disk>();
        foreach (var d in allDisks)
            d.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    private void CheckWin()
    {
        // Победа: все 3 диска ОДНОВРЕМЕННО на 3 столбе
        if (peg3.disks.Count == 3)
        {
            if (winPanel != null)
                winPanel.SetActive(true);
            
            if (winMovesText != null)
                winMovesText.text = "Moves: " + moves;

            // чтобы игрок не мог трогать диски после победы
            BlockAllDisks();
        }
    }
}

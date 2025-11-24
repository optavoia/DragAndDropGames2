using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class RewardedAdsHanoi : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "RewardedHanoi_Android";
    string _adUnitId;

    [SerializeField] Button _hanoiButton;

    private GameManagerUI game;

    private bool adLoaded = false;    // <── ДОБАВЛЕНО

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _adUnitId = _androidAdUnitId;
    }

    private void OnEnable()
    {
        TryFindButton();
        TryFindHanoiGame();
        LoadAd();  // << ДОБАВИТЬ ЭТО

        // если реклама уже загружена → включаем кнопку
        if (adLoaded && _hanoiButton != null)
            _hanoiButton.interactable = true;
    }

    private void TryFindButton()
    {
        StartCoroutine(TryFindButtonRoutine());
    }

    IEnumerator TryFindButtonRoutine()
    {
        while (_hanoiButton == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("RewardedHanoiButton");
            if (obj != null)
            {
                Button btn = obj.GetComponent<Button>();
                if (btn != null)
                {
                    SetButton(btn);

                    if (adLoaded)
                        _hanoiButton.interactable = true;

                    Debug.Log("RewardedAdsHanoi: Button FOUND!");
                    yield break;
                }
            }

            Debug.Log("RewardedAdsHanoi: Button NOT FOUND, retry...");
            yield return new WaitForSeconds(0.3f);
        }
    }


    private void TryFindHanoiGame()
    {
        game = FindAnyObjectByType<GameManagerUI>();
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
            return;

        Advertisement.Load(_adUnitId, this);
    }

    public void SetButton(Button btn)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(ShowAd);
        _hanoiButton = btn;
        _hanoiButton.interactable = false;
    }

    public void ShowAd()
    {
        _hanoiButton.interactable = false;
        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId != _adUnitId) return;

        Debug.Log("HANOI rewarded ad LOADED!");

        adLoaded = true;  // <── ЗАПОМНИЛИ

        if (_hanoiButton != null)
            _hanoiButton.interactable = true;  // <── НЕ будет ошибки
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Time.timeScale = 0f;
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Time.timeScale = 1f;

        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            TryFindHanoiGame();
            if (game != null)
                game.freeMoves = 3;
        }

        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        adLoaded = false;   // сброс
        yield return new WaitForSeconds(3f);
        LoadAd();
    }

    public void OnUnityAdsShowFailure(string p, UnityAdsShowError e, string m)
    {
        StartCoroutine(Reload());
    }

    public void OnUnityAdsFailedToLoad(string p, UnityAdsLoadError e, string m)
    {
        StartCoroutine(Reload());
    }

    public void OnUnityAdsShowClick(string p) { }
}

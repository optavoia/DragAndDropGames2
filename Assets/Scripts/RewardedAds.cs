using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    string _adUnitId;

    [SerializeField] Button _rewardedAdButton;

    private float slowDownDuration = 30f;
    private bool adLoaded = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _adUnitId = _androidAdUnitId;
    }

    // ---------------- LOAD ----------------

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Rewarded ad tried to load before Unity Ads initialized!");
            return;
        }

        Debug.Log("Loading rewarded ad");
        Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId != _adUnitId) return;

        Debug.Log("Rewarded ad loaded!");

        // кнопка может ещё НЕ быть найдена
        if (_rewardedAdButton != null)
        {
            _rewardedAdButton.interactable = true;
        }
        else
        {
            Debug.LogWarning("RewardedAds: Ad loaded BUT button is NULL! Will try to find again.");
            TryFindButtonInScene(); // <<<<<<<<<< ДОБАВЛЕНО
        }

    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning("Rewarded ad failed to load!");
        StartCoroutine(WaitAndReload(5f));
    }

    IEnumerator WaitAndReload(float time)
    {
        yield return new WaitForSeconds(time);
        LoadAd();
    }

    // ---------------- SHOW ----------------

    public void ShowAd()
    {
        _rewardedAdButton.interactable = false;
        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Rewarded ad started!");
        Time.timeScale = 0f;  // пауза как в interstitial
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("Rewarded ad clicked!");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning("Rewarded ad show failed!");
        StartCoroutine(WaitAndReload(5f));
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Rewarded ad completed!");

        // восстановить нормальную скорость
        Time.timeScale = 1f;

        // замедлить игру на 30 секунд (как interstitial)
        StartCoroutine(SlowDownForTime(slowDownDuration));

        // перезагрузить
        StartCoroutine(WaitAndReload(10f));
    }

    // ---------------- TIME ----------------

    private IEnumerator SlowDownForTime(float seconds)
    {
        Time.timeScale = 0.4f;

        float timer = 0f;
        while (timer < seconds)
        {
            // если сменили сцену -> выйти
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "CityScene")
            {
                Time.timeScale = 1f;
                yield break;
            }

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f;
    }

    // ---------------- BUTTON ----------------

    public void SetButton(Button btn)
    {
        if (btn == null) return;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(ShowAd);
        _rewardedAdButton = btn;

        _rewardedAdButton.interactable = false;
    }

    private void OnEnable()
    {
        TryFindButtonInScene();

        if (adLoaded && _rewardedAdButton != null)
            _rewardedAdButton.interactable = true;
    }

    private void TryFindButtonInScene()
    {
        GameObject btnObj = GameObject.FindGameObjectWithTag("RewardedButton");
        if (btnObj == null)
        {
            Debug.LogWarning("RewardedAds: RewardedButton NOT FOUND in scene!");
            return;
        }

        Button btn = btnObj.GetComponent<Button>();
        if (btn == null)
        {
            Debug.LogError("RewardedAds: Button object found but NO Button component!");
            return;
        }

        SetButton(btn);
        Debug.Log("RewardedAds: Button linked successfully!");
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class RewardedAdsHanoi : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("Unity Ads Placement ID")]
    [SerializeField] private string _androidAdUnitId = "Rewarded_Hanoi";
    private string _adUnitId;

    [Header("Button (auto-find by tag)")]
    [SerializeField] private Button rewardedHanoiButton;

    private bool adLoaded = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _adUnitId = _androidAdUnitId;
    }

    private void Start()
    {
        // пробуем найти кнопку сразу
        TryFindButton();
    }

    // ========================== LOAD ==========================

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("[HANOI] Ads not initialized yet!");
            return;
        }

        Debug.Log("[HANOI] Loading rewarded ad...");
        Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId != _adUnitId) return;

        Debug.Log("[HANOI] Ad loaded!");
        adLoaded = true;

        // пробуем снова найти кнопку (если не нашли раньше)
        TryFindButton();
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning("[HANOI] Failed to load, retry...");
        StartCoroutine(ReloadDelay());
    }

    IEnumerator ReloadDelay()
    {
        yield return new WaitForSeconds(3f);
        LoadAd();
    }

    // ========================== SHOW ==========================

    public void ShowAd()
    {
        if (!adLoaded)
        {
            Debug.LogWarning("[HANOI] Ad is not loaded yet!");
            return;
        }

        if (rewardedHanoiButton != null)
            rewardedHanoiButton.interactable = false;

        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Time.timeScale = 0f;
        Debug.Log("[HANOI] Show started!");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning("[HANOI] Show failed!");
        Time.timeScale = 1f;
        StartCoroutine(ReloadDelay());
    }

    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState state)
    {
        Time.timeScale = 1f;

        if (state == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("[HANOI] Ad completed -> reward player");

            GameManagerUI gm = FindObjectOfType<GameManagerUI>();

            if (gm != null)
                gm.AddFreeMoves(3);
            else
                Debug.LogError("[HANOI] GameManagerUI not found!");
        }

        StartCoroutine(ReloadDelay());
    }

    // ========================== BUTTON ==========================

    private void TryFindButton()
    {
        if (rewardedHanoiButton != null)
            return;

        Debug.Log("[HANOI] Searching for button with tag RewardedHanoiButton...");

        GameObject obj = GameObject.FindGameObjectWithTag("RewardedHanoiButton");

        if (obj == null)
        {
            Debug.LogWarning("[HANOI] Button NOT FOUND. Will try again in 0.5s.");
            StartCoroutine(TryFindLater());
            return;
        }

        Button btn = obj.GetComponent<Button>();

        if (btn == null)
        {
            Debug.LogError("[HANOI] Object found but has NO Button component!");
            return;
        }

        rewardedHanoiButton = btn;
        rewardedHanoiButton.onClick.RemoveAllListeners();
        rewardedHanoiButton.onClick.AddListener(ShowAd);

        rewardedHanoiButton.interactable = adLoaded;

        Debug.Log("[HANOI] Button linked successfully!");
    }

    IEnumerator TryFindLater()
    {
        yield return new WaitForSeconds(0.5f);
        TryFindButton();
    }
}

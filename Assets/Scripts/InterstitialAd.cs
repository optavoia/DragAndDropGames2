using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    string _adUnitId;

    public event Action OnInterstitialAdReady;
    public bool isReady = false;
    [SerializeField] Button _interstitialAdButton;
    private bool isManualAd = false;  // новый флаг

    void Awake()
    {
        _adUnitId = _androidAdUnitId;
    }

    private void Update()
    {
        if (AdManager.Instance != null && AdManager.Instance.interstitialAd != null)
        {
            _interstitialAdButton.interactable = isReady;
        }
    }

    public void OnInterstitialAdButtonClicked()
    {
        Debug.Log("Interstitial ad button clicked!");
        ShowInterstitial();
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Tried to load interstitial ad before Unity ads was initialized!");
            return;
        }

        Debug.Log("Loading interstitial ad");
        Advertisement.Load(_adUnitId, this);
    }

    public void ShowAd()
    {
        if (isReady)
        {
            // 🔽 Перед показом — скрываем баннер
            AdManager.Instance.bannerAd?.HideBanner();

            Advertisement.Show(_adUnitId, this);
            isReady = false;
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not ready yet!");
            LoadAd();
        }
    }

    public void ShowInterstitial()
    {
        if (AdManager.Instance.interstitialAd != null && isReady)
        {
            Debug.Log("Showing interstitial ad manually!");
            isManualAd = true;
            ShowAd();

        }
        else
        {
            Debug.Log("Interstitial ad not ready yet, loading again!");
            LoadAd();
        }
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Interstitial ad loaded!");
        isReady = true;

        if (_interstitialAdButton != null)
            _interstitialAdButton.interactable = true;

        OnInterstitialAdReady?.Invoke();
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning("Failed to load interstitial ad!");
        LoadAd();
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("User clicked on interstitial ad!");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Interstitial ad finished");

        Time.timeScale = 1f;   // просто возвращаем норму

        isManualAd = false;
        LoadAd();
        AdManager.Instance.bannerAd?.ShowBanner();
        
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log("Error showing interstitial ad!");
        LoadAd();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Showing interstitial ad at this moment!");
        Time.timeScale = 0f; // ❌ останавливаем игру
    }

    public void SetButton(Button button)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnInterstitialAdButtonClicked);
        _interstitialAdButton = button;
        _interstitialAdButton.interactable = false;
    }

    private void OnEnable()
    {
        TryFindButton();

        if (isReady && _interstitialAdButton != null)
            _interstitialAdButton.interactable = true;
    }

    private void TryFindButton()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("AdAddButton");
        if (obj == null) return;

        Button btn = obj.GetComponent<Button>();
        if (btn == null) return;

        SetButton(btn);

        if (isReady)
            _interstitialAdButton.interactable = true;
    }
}
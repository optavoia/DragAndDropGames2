using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class AdManager : MonoBehaviour
{
    public AdsInitializer adsInitializer;
    public InterstitialAd interstitialAd;
    [SerializeField] bool turnOffInterstitialAd = false;
    private bool firstAdShown = false;

    public BannerAd bannerAd;
    [SerializeField] bool turnOffBannerAd = false;

    public RewardedAds rewardedAds;
    [SerializeField] bool turnOffRewardedAd = false;
    // .......

    public static AdManager Instance { get; private set; }


    private void Awake()
    {
        if (adsInitializer == null)
            adsInitializer = FindFirstObjectByType<AdsInitializer>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        adsInitializer.OnAdsInitialized += HandleAdsInitialized;
    }

    private void HandleAdsInitialized()
    {
        if (!turnOffInterstitialAd)
        {
            interstitialAd.OnInterstitialAdReady += HandleInterstitialReady;
            interstitialAd.LoadAd();
        }

        if (!turnOffBannerAd && bannerAd != null)
        {
            bannerAd.LoadBanner();
        }

        if (!turnOffRewardedAd)
        {
            rewardedAds.LoadAd();
        }
    }


    private void HandleInterstitialReady()
    {
        if (!firstAdShown)
        {
            Debug.Log("Showing first time interstitial ad automatically!");
            interstitialAd.ShowAd();
            firstAdShown = true;

        }
        else
        {
            Debug.Log("Next interstitial ad is ready for manual show!");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private bool firstSceneLoad = false;    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (interstitialAd == null)
            interstitialAd = FindFirstObjectByType<InterstitialAd>();

        Button interstitialButton =
            GameObject.FindGameObjectWithTag("AdAddButton")?.GetComponent<Button>();

        if (interstitialAd != null && interstitialButton != null)
        {
            interstitialAd.SetButton(interstitialButton);
        }

        if (rewardedAds == null)
            rewardedAds = FindFirstObjectByType<RewardedAds>();

        Button rewardedAdButton
            = GameObject.FindGameObjectWithTag("RewardedButton").GetComponent<Button>();

        if (rewardedAds != null && rewardedAdButton != null)
        {
            rewardedAds.SetButton(rewardedAdButton);
        }

        // При первом запуске — просто пропускаем
        if (!firstSceneLoad)
        {
            firstSceneLoad = true;
            Debug.Log("First scene loaded (startup) — skip ad.");
            return;
        }

        // На всех остальных сценах показываем рекламу
        Debug.Log($"Scene '{scene.name}' loaded! Showing interstitial ad...");

        if (interstitialAd != null && interstitialAd.isReady)
        {
            interstitialAd.ShowAd();
        }
        else
        {
            Debug.Log("Interstitial not ready yet, loading new one...");
            interstitialAd?.LoadAd();
        }

       
    }

}
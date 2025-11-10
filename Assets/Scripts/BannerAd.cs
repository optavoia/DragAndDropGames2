using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour
{
    [SerializeField] string _androidAdUnitId = "Banner_Android";
    private string _adUnitId;
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    void Awake()
    {
#if UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif
        // Устанавливаем позицию баннера (например, снизу)
        Advertisement.Banner.SetPosition(_bannerPosition);
    }

    public void LoadBanner()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.LogWarning("Trying to load banner before Unity Ads initialized!");
            return;
        }

        Debug.Log("Loading banner ad...");
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(_adUnitId, options);
    }

    private void OnBannerLoaded()
    {
        Debug.Log("Banner loaded successfully!");
        ShowBanner();
    }

    private void OnBannerError(string message)
    {
        Debug.LogWarning($"Banner failed to load: {message}");
        // Попробуем перезагрузить через несколько секунд
        Invoke(nameof(LoadBanner), 5f);
    }

    public void ShowBanner()
    {
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        Advertisement.Banner.Show(_adUnitId, options);
    }

    public void HideBanner()
    {
        Debug.Log("Hiding banner...");
        Advertisement.Banner.Hide();
    }

    private void OnBannerClicked() => Debug.Log("Banner clicked!");
    private void OnBannerShown() => Debug.Log("Banner shown!");
    private void OnBannerHidden() => Debug.Log("Banner hidden!");
}

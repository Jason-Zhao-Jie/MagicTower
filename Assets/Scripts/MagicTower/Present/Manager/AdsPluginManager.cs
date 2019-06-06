using GoogleMobileAds.Api;

namespace MagicTower.Present.Manager
{

    public static class AdsPluginManager
    {
        public enum AdLoadingState
        {
            OK,
            UnablePlatform,
            SdkInitializing,
            NotInitialized,
            Loading,
            LoadFailedAndReloading,
        }

        private static bool DEBUG => Game.IsDebug && true;  // TODO : 去掉结尾的 && true 以结束强制广告测试

        private const string appid_android = "ca-app-pub-7886607022943212~3839402738";
        public const string appid_ios = "ca-app-pub-7886607022943212~5320360588";

        private const string testBannerId_android = "ca-app-pub-3940256099942544/6300978111";
        private const string testBannerId_ios = "ca-app-pub-3940256099942544/2934735716";

        private const string bannerid_android = "";
        private const string bannerid_ios = "";

        private const string testInterstitialId_android = "ca-app-pub-3940256099942544/1033173712";
        private const string testInterstitialId_ios = "ca-app-pub-3940256099942544/4411468910";

        private const string interstitialid_android = "ca-app-pub-7886607022943212/2540679227";
        private const string interstitialid_ios = "ca-app-pub-7886607022943212/7928737273";

        private const string testRewardId_android = "ca-app-pub-3940256099942544/5224354917";
        private const string testRewardId_ios = "ca-app-pub-3940256099942544/1712485313";

        private const string rewardid_android = "ca-app-pub-7886607022943212/7782872396";
        private const string rewardid_ios = "ca-app-pub-7886607022943212/9614828818";

        private static readonly string[] test_device_id = new string[] {
            "B880A9E1584344957D3254733A14AD73",
            "0B5B8903B4DD0F0A99AEC7D431E5CBF8",
        };

        public static void Initialize(bool needTopBanner, bool needInterstitial)
        {
            if (SdkInitialized)
            {
                return;
            }
            switch (UnityEngine.Application.platform)
            {
                case UnityEngine.RuntimePlatform.Android:
                    MobileAds.Initialize(appid_android);
                    SdkInitialized = true;
                    break;
                case UnityEngine.RuntimePlatform.IPhonePlayer:
                    MobileAds.Initialize(appid_ios);
                    SdkInitialized = true;
                    break;
            }
            if (SdkInitialized)
            {
                if (needTopBanner)
                {
                    InitializeTopBanner();
                }
                if (needInterstitial)
                {
                    InitializeInterstitial();
                }
                InitializeRewardBaseVideo();
            }
        }

        public static bool SdkInitialized { get; private set; }

        private static AdRequest.Builder CreateRequestBuilder()
        {
            // 创建广告请求器，并标记为所有年龄段分级 (G)
            var requestBuilder = new AdRequest.Builder().AddExtra("max_ad_content_rating", "G");
            if (test_device_id.Length > 0)
            {
                foreach (var i in test_device_id)
                {
                    requestBuilder = requestBuilder.AddTestDevice(i);
                }
            }
            return requestBuilder;
        }

        private static Model.EGameStatus lastStatus;


        //////////////////////// Top Banner /////////////////////////// 

        public static string TopBannerLoadedFailedMessage { get; private set; }

        public static AdLoadingState TopBannerState
        {
            get
            {
                if (!SdkInitialized)
                {
                    return UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android
                        || UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer
                        ? AdLoadingState.SdkInitializing : AdLoadingState.UnablePlatform;
                }
                if (bannerView == null)
                {
                    return AdLoadingState.NotInitialized;
                }
                return topBannerLoadedFailed ? AdLoadingState.LoadFailedAndReloading : AdLoadingState.OK;
            }
        }

        public static AdLoadingState ShowTopBanner(Model.EmptyCallBack closeCB)
        {
            if (TopBannerState == AdLoadingState.OK)
            {
                bannerClosedCallback = closeCB;
                bannerView.Show();
            }
            return TopBannerState;
        }

        private static AdLoadingState InitializeTopBanner()
        {
            switch (UnityEngine.Application.platform)
            {
                case UnityEngine.RuntimePlatform.Android:
                    if (DEBUG)
                    {
                        bannerView = new BannerView(testBannerId_android, AdSize.Banner, AdPosition.Top);
                    }
                    else
                    {
                        bannerView = new BannerView(bannerid_android, AdSize.Banner, AdPosition.Top);
                    }
                    break;
                case UnityEngine.RuntimePlatform.IPhonePlayer:
                    MobileAds.Initialize(appid_ios);
                    if (DEBUG)
                    {
                        bannerView = new BannerView(testBannerId_ios, AdSize.Banner, AdPosition.Top);
                    }
                    else
                    {
                        bannerView = new BannerView(bannerid_ios, AdSize.Banner, AdPosition.Top);
                    }
                    SdkInitialized = true;
                    break;
            }
            if (bannerView != null)
            {
                bannerView.OnAdFailedToLoad += OnTopBannerFailedToLoad;
                bannerView.OnAdClosed += OnTopBannerClosed;
                bannerView.OnAdLoaded += OnTopBannerLoaded;
                bannerView.LoadAd(CreateRequestBuilder().Build());
            }
            return TopBannerState;
        }

        private static void OnTopBannerFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            topBannerLoadedFailed = true;
            TopBannerLoadedFailedMessage = e.Message;
            bannerView.LoadAd(CreateRequestBuilder().Build());
        }

        private static void OnTopBannerClosed(object sender, System.EventArgs e)
        {
            Game.Status = lastStatus;
            bannerView.LoadAd(CreateRequestBuilder().Build());
            Game.InvokeInMainThread(bannerClosedCallback);
            bannerClosedCallback = null;
        }

        private static void OnTopBannerLoaded(object sender, System.EventArgs e)
        {
            topBannerLoadedFailed = false;
        }

        private static bool topBannerLoadedFailed = false;
        private static BannerView bannerView = null;
        private static Model.EmptyCallBack bannerClosedCallback = null;


        //////////////////////// Interstitial /////////////////////////// 

        public static string InterstitialLoadedFailedMessage { get; private set; }

        public static AdLoadingState InterstitialState
        {
            get
            {
                if (!SdkInitialized)
                {
                    return UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android
                        || UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer
                        ? AdLoadingState.SdkInitializing : AdLoadingState.UnablePlatform;
                }
                if (interstitialView == null)
                {
                    return AdLoadingState.NotInitialized;
                }
                if (interstitialView.IsLoaded())
                {
                    return AdLoadingState.OK;
                }
                return interstitialLoadedFailed ? AdLoadingState.LoadFailedAndReloading : AdLoadingState.Loading;
            }
        }

        public static AdLoadingState ShowInterstitial(Model.EmptyCallBack closeCB)
        {
            if (InterstitialState == AdLoadingState.OK)
            {
                lastStatus = Game.Status;
                Game.Status = Model.EGameStatus.OnPlayingAds;
                interstitialClosedCallback = closeCB;
                interstitialView.Show();
            }
            return InterstitialState;
        }

        private static AdLoadingState InitializeInterstitial()
        {
            if (interstitialView == null)
            {
                switch (UnityEngine.Application.platform)
                {
                    case UnityEngine.RuntimePlatform.Android:
                        if (DEBUG)
                        {
                            interstitialView = new InterstitialAd(testInterstitialId_android);
                        }
                        else
                        {
                            interstitialView = new InterstitialAd(interstitialid_android);
                        }
                        break;
                    case UnityEngine.RuntimePlatform.IPhonePlayer:
                        MobileAds.Initialize(appid_ios);
                        if (DEBUG)
                        {
                            interstitialView = new InterstitialAd(testInterstitialId_ios);
                        }
                        else
                        {
                            interstitialView = new InterstitialAd(interstitialid_ios);
                        }
                        break;
                }
            }
            if (interstitialView != null)
            {
                interstitialView.OnAdFailedToLoad += OnInterstitialFailedToLoad;
                interstitialView.OnAdClosed += OnInterstitialClosed;
                interstitialView.OnAdLoaded += OnInterstitialLoaded;
                interstitialView.LoadAd(CreateRequestBuilder().Build());
            }
            return InterstitialState;
        }

        private static void OnInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            interstitialLoadedFailed = true;
            InterstitialLoadedFailedMessage = e.Message;
            interstitialView.LoadAd(CreateRequestBuilder().Build());
        }

        private static void OnInterstitialClosed(object sender, System.EventArgs e)
        {
            Game.Status = lastStatus;
            interstitialView.LoadAd(CreateRequestBuilder().Build());
            Game.InvokeInMainThread(interstitialClosedCallback);
            interstitialClosedCallback = null;
        }

        private static void OnInterstitialLoaded(object sender, System.EventArgs e)
        {
            interstitialLoadedFailed = false;
        }

        private static bool interstitialLoadedFailed = false;
        private static InterstitialAd interstitialView = null;
        private static Model.EmptyCallBack interstitialClosedCallback = null;


        //////////////////////// Reward Base Video /////////////////////////// 

        public static string RewardBasedVideoLoadedFailedMessage { get; private set; }
        public delegate void RewardCallback(string type, double amount);

        public static AdLoadingState RewardBaseVideoState
        {
            get
            {
                if (!SdkInitialized)
                {
                    return UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android
                        || UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer
                        ? AdLoadingState.SdkInitializing : AdLoadingState.UnablePlatform;
                }
                if (RewardBasedVideoAd.Instance.IsLoaded())
                {
                    return AdLoadingState.OK;
                }
                return rewardBasedVideoLoadedFailed ? AdLoadingState.LoadFailedAndReloading : AdLoadingState.Loading;
            }
        }

        public static AdLoadingState ShowRewardBasedVideo(RewardCallback rewardCB, Model.EmptyCallBack closeCB)
        {
            if (RewardBaseVideoState == AdLoadingState.OK)
            {
                lastStatus = Game.Status;
                Game.Status = Model.EGameStatus.OnPlayingAds;
                rewardCallback = rewardCB;
                rewardbasedClosedCallback = closeCB;
                RewardBasedVideoAd.Instance.Show();
            }
            return RewardBaseVideoState;
        }

        private static AdLoadingState InitializeRewardBaseVideo()
        {
            if (SdkInitialized)
            {
                RewardBasedVideoAd.Instance.OnAdRewarded += OnRewardCall;
                RewardBasedVideoAd.Instance.OnAdFailedToLoad += OnRewardBasedVideoFailedToLoad;
                RewardBasedVideoAd.Instance.OnAdClosed += OnRewardBasedVideoClosed;
                RewardBasedVideoAd.Instance.OnAdLoaded += OnRewardBasedVideoLoaded;
                switch (UnityEngine.Application.platform)
                {
                    case UnityEngine.RuntimePlatform.Android:
                        if (DEBUG)
                        {
                            RewardBasedVideoAd.Instance.LoadAd(CreateRequestBuilder().Build(), testRewardId_android);
                        }
                        else
                        {
                            RewardBasedVideoAd.Instance.LoadAd(CreateRequestBuilder().Build(), rewardid_android);
                        }
                        break;
                    case UnityEngine.RuntimePlatform.IPhonePlayer:
                        if (DEBUG)
                        {
                            RewardBasedVideoAd.Instance.LoadAd(CreateRequestBuilder().Build(), testRewardId_ios);
                        }
                        else
                        {
                            RewardBasedVideoAd.Instance.LoadAd(CreateRequestBuilder().Build(), rewardid_ios);
                        }
                        break;
                }
            }
            return RewardBaseVideoState;
        }

        private static void OnRewardCall(object sender, Reward args)
        {
            Game.Status = lastStatus;
            Game.InvokeInMainThread(() =>
            {
                rewardCallback?.Invoke(args.Type, args.Amount);
            });
            rewardbasedClosedCallback = null;
        }

        private static void OnRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            rewardBasedVideoLoadedFailed = true;
            RewardBasedVideoLoadedFailedMessage = e.Message;
            switch (UnityEngine.Application.platform)
            {
                case UnityEngine.RuntimePlatform.Android:
                    if (DEBUG)
                    {
                        RewardBasedVideoAd.Instance.LoadAd(CreateRequestBuilder().Build(), testRewardId_android);
                    }
                    else
                    {
                        RewardBasedVideoAd.Instance.LoadAd(CreateRequestBuilder().Build(), rewardid_android);
                    }
                    break;
                case UnityEngine.RuntimePlatform.IPhonePlayer:
                    if (DEBUG)
                    {
                        RewardBasedVideoAd.Instance.LoadAd(CreateRequestBuilder().Build(), testRewardId_ios);
                    }
                    else
                    {
                        RewardBasedVideoAd.Instance.LoadAd(CreateRequestBuilder().Build(), rewardid_ios);
                    }
                    break;
            }
        }

        private static void OnRewardBasedVideoClosed(object sender, System.EventArgs e)
        {
            Game.Status = lastStatus;
            switch (UnityEngine.Application.platform)
            {
                case UnityEngine.RuntimePlatform.Android:
                    if (DEBUG)
                    {
                        RewardBasedVideoAd.Instance.LoadAd(CreateRequestBuilder().Build(), testRewardId_android);
                    }
                    else
                    {
                        RewardBasedVideoAd.Instance.LoadAd(CreateRequestBuilder().Build(), rewardid_android);
                    }
                    break;
                case UnityEngine.RuntimePlatform.IPhonePlayer:
                    if (DEBUG)
                    {
                        RewardBasedVideoAd.Instance.LoadAd(CreateRequestBuilder().Build(), testRewardId_ios);
                    }
                    else
                    {
                        RewardBasedVideoAd.Instance.LoadAd(CreateRequestBuilder().Build(), rewardid_ios);
                    }
                    break;
            }
            Game.InvokeInMainThread(rewardbasedClosedCallback);
            rewardbasedClosedCallback = null;
        }

        private static void OnRewardBasedVideoLoaded(object sender, System.EventArgs e)
        {
            rewardBasedVideoLoadedFailed = false;
        }

        private static bool rewardBasedVideoLoadedFailed = false;
        private static RewardCallback rewardCallback = null;
        private static Model.EmptyCallBack rewardbasedClosedCallback = null;
    }

}

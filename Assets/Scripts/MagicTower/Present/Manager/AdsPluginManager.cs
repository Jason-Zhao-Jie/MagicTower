using GoogleMobileAds.Api;

namespace MagicTower.Present.Manager
{

    public static class AdsPluginManager
    {
        public enum AdType
        {
            BannerTop,
            BannerBottom,
            BannerMid,
            Interstitial,
            RewardBased,
        }

        public enum Result
        {
            Successful,
            UnablePlatform,
            NotInitialized,
            InitializeFailure,
            NotLoaded,
        }

        public delegate void RewardCallback(string type, double amount);
        public delegate void AdFailedCallback(AdType type, string message);

        private const bool DEBUG = Game.DEBUG && true;  // 去掉结尾的 && true 以结束强制广告测试

        private const string appid_android = "ca-app-pub-7886607022943212~3839402738";
        private const string appid_ios = "ca-app-pub-7886607022943212~5320360588";

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

        public static void Initialize(bool needBanner, bool needInterstitial)
        {
            if (Initialized)
            {
                return;
            }
            switch (UnityEngine.Application.platform)
            {
                case UnityEngine.RuntimePlatform.Android:
                    MobileAds.Initialize(appid_android);
                    if (DEBUG)
                    {
                        if (needBanner)
                        {
                            bannerView = new BannerView(testBannerId_android, AdSize.Banner, AdPosition.Top);
                        }
                        if (needInterstitial)
                        {
                            interstitialView = new InterstitialAd(testInterstitialId_android);
                        }
                    }
                    else
                    {
                        if (needBanner)
                        {
                            bannerView = new BannerView(bannerid_android, AdSize.Banner, AdPosition.Top);
                        }
                        if (needInterstitial)
                        {
                            interstitialView = new InterstitialAd(interstitialid_android);
                        }
                    }
                    Initialized = true;
                    break;
                case UnityEngine.RuntimePlatform.IPhonePlayer:
                    MobileAds.Initialize(appid_ios);
                    if (DEBUG)
                    {
                        if (needBanner)
                        {
                            bannerView = new BannerView(testBannerId_ios, AdSize.Banner, AdPosition.Top);
                        }
                        if (needInterstitial)
                        {
                            interstitialView = new InterstitialAd(testInterstitialId_ios);
                        }
                    }
                    else
                    {
                        if (needBanner)
                        {
                            bannerView = new BannerView(bannerid_ios, AdSize.Banner, AdPosition.Top);
                        }
                        if (needInterstitial)
                        {
                            interstitialView = new InterstitialAd(interstitialid_ios);
                        }
                    }
                    Initialized = true;
                    break;
            }

            if (Initialized)
            {
                if (bannerView != null)
                {
                    bannerView.OnAdFailedToLoad += (sender, e) => {
                        Game.InvokeInMainThread(() => { AdFailedCB?.Invoke(AdType.BannerTop, e.Message); });
                    };
                    bannerView.OnAdClosed += (sender, e) => {
                        OnAdClosedCall(AdType.BannerTop);
                    };
                    bannerView.LoadAd(CreateRequestBuilder().Build());
                }
                if (interstitialView != null)
                {
                    interstitialView.OnAdFailedToLoad += (sender, e) => {
                        Game.InvokeInMainThread(() => { AdFailedCB?.Invoke(AdType.Interstitial, e.Message); });
                    };
                    interstitialView.OnAdClosed += (sender, e) => {
                        OnAdClosedCall(AdType.Interstitial);
                    };
                    interstitialView.LoadAd(CreateRequestBuilder().Build());
                }

                RewardBasedVideoAd.Instance.OnAdRewarded += OnRewardCall;
                RewardBasedVideoAd.Instance.OnAdFailedToLoad += (sender, e) => {
                    Game.InvokeInMainThread(() => { AdFailedCB?.Invoke(AdType.RewardBased, e.Message); });
                };
                RewardBasedVideoAd.Instance.OnAdClosed += (sender, e) => {
                    OnAdClosedCall(AdType.RewardBased);
                };
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
        }

        public static bool Initialized { get; private set; }

        public static AdFailedCallback AdFailedCB { get; set; }

        public static Result ShowBanner(Model.EmptyCallBack closeCB)
        {
            if (!Initialized)
            {
                return UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android || UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer ? Result.NotInitialized : Result.UnablePlatform;
            }
            if (bannerView == null)
            {
                return Result.InitializeFailure;
            }
            bannerClosedCallback = closeCB;
            bannerView.Show();
            return Result.Successful;
        }

        public static Result ShowInterstitial(Model.EmptyCallBack closeCB)
        {
            if (!Initialized)
            {
                return UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android || UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer ? Result.NotInitialized : Result.UnablePlatform;
            }
            if (interstitialView == null)
            {
                return Result.InitializeFailure;
            }
            if (!interstitialView.IsLoaded())
            {
                return Result.NotLoaded;
            }
            lastStatus = Game.Status;
            Game.Status = Model.EGameStatus.OnPlayingAds;
            interstitialClosedCallback = closeCB;
            interstitialView.Show();
            return Result.Successful;
        }

        public static Result ShowRewardBasedVideo(RewardCallback rewardCB, Model.EmptyCallBack closeCB)
        {
            if (!Initialized)
            {
                return UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android || UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer ? Result.NotInitialized : Result.UnablePlatform;
            }
            if (!RewardBasedVideoAd.Instance.IsLoaded())
            {
                return Result.NotLoaded;
            }
            lastStatus = Game.Status;
            Game.Status = Model.EGameStatus.OnPlayingAds;
            rewardCallback = rewardCB;
            rewardbasedClosedCallback = closeCB;
            RewardBasedVideoAd.Instance.Show();
            return Result.Successful;
        }

        public static AdRequest.Builder CreateRequestBuilder()
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

        private static void OnRewardCall(object sender, Reward args)
        {
            Game.Status = lastStatus;
            Game.InvokeInMainThread(() =>
            {
                rewardCallback?.Invoke(args.Type, args.Amount);
            });
            rewardbasedClosedCallback = null;
        }

        private static void OnAdClosedCall(AdType type)
        {
            Game.Status = lastStatus;
            switch (type)
            {
                case AdType.BannerTop:
                    bannerView.LoadAd(CreateRequestBuilder().Build());
                    Game.InvokeInMainThread(bannerClosedCallback);
                    bannerClosedCallback = null;
                    break;
                case AdType.Interstitial:
                    interstitialView.LoadAd(CreateRequestBuilder().Build());
                    Game.InvokeInMainThread(interstitialClosedCallback);
                    interstitialClosedCallback = null;
                    break;
                case AdType.RewardBased:
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
                    break;
            }
        }

        private static BannerView bannerView = null;
        private static InterstitialAd interstitialView = null;
        private static RewardCallback rewardCallback = null;
        private static Model.EmptyCallBack bannerClosedCallback = null;
        private static Model.EmptyCallBack interstitialClosedCallback = null;
        private static Model.EmptyCallBack rewardbasedClosedCallback = null;

        private static Model.EGameStatus lastStatus;
    }

}

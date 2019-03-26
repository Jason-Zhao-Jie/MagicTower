namespace MagicTower.Present.Manager
{

    public static class AdsPluginManager
    {

        private const string googleMobAdsAppid_android = "ca-app-pub-7886607022943212~3839402738";
        private const string googleMobAdsAppid_ios = "ca-app-pub-7886607022943212~5320360588";

        static AdsPluginManager()
        {
            switch (UnityEngine.Application.platform)
            {
                case UnityEngine.RuntimePlatform.Android:
                    GoogleMobileAds.Api.MobileAds.Initialize(googleMobAdsAppid_android);
                    break;
                case UnityEngine.RuntimePlatform.IPhonePlayer:
                    GoogleMobileAds.Api.MobileAds.Initialize(googleMobAdsAppid_ios);
                    break;
            }
        }
    }

}

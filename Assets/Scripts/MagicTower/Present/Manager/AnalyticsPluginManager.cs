using Firebase.Analytics;
using UnityEngine.Analytics;

namespace MagicTower.Present.Manager
{
    public static class AnalyticsPluginManager
    {
        public static void Initialize()
        {
            var userId = AnalyticsSessionInfo.userId;
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            Analytics.enabled = true;
            FirebaseAnalytics.SetUserId(userId);
            Analytics.SetUserId(userId);
            Analytics.deviceStatsEnabled = true;
        }

        public static void SetUserProperty(string name, string value) {
            FirebaseAnalytics.SetUserProperty(name, value);
            Analytics.CustomEvent("UserProperty", new System.Collections.Generic.Dictionary<string, object> { { name, value } });
        }

        public static void LogEvent(EventType type)
        {
            FirebaseAnalytics.LogEvent("custom_" + type.ToString());
            switch (type)
            {
                case EventType.GameStart:
                    AnalyticsEvent.GameStart();
                    break;
                default:
                    AnalyticsEvent.CustomEvent();
                    break;
            }
        }

        public enum EventType {
            GameStart,
        }
    }
}

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_IPHONE
using UnityEditor.iOS.Xcode;
#endif

public static class PListProcessor
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
#if UNITY_IPHONE
          string plistPath = Path.Combine(path, "Info.plist");
          PlistDocument plist = new PlistDocument();

          plist.ReadFromFile(plistPath); 
          plist.root.SetString("GADApplicationIdentifier", MagicTower.Present.Manager.AdsPluginManager.appid_ios);
          plist.root.SetBoolean("GADIsAdManagerApp", true);
          File.WriteAllText(plistPath, plist.WriteToString());
#endif
    }
}

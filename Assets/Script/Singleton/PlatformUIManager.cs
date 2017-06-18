
public static class PlatformUIManager
{
    public static bool ShowMessageBox(string message)
    {
        if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsEditor || UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsPlayer)
            ;//System.Windows.Forms.MessageBox.Show(message);
        return true;
    }
}

using UnityEditor;

public class MainMenuExtension
{
    [MenuItem("ArmyAnt/Menu Test")]
    public static void MenuTest() {
        EditorUtility.DisplayDialog("Menu Test", "Test Successful!", "OK");
    }
}

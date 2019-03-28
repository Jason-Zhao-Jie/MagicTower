using UnityEngine;
using System.Collections;

namespace MagicTower.Components
{
    public enum SceneType : byte
    {
        None,
        StartScene,
        MainScene,
        DataEditorScene,
    }

    public abstract class AScene : MonoBehaviour
    {
        public abstract SceneType Type { get; }

        // Use this for initialization
        virtual protected System.Threading.Tasks.Task Start()
        {
            Game.Initial(this);
            return null;
        }

        virtual protected void OnApplicationQuit()
        {
            Game.ObjPool.ClearAll();
            Game.Map?.ClearMap();
            Resources.UnloadUnusedAssets();
        }

        virtual protected void OnDestroy()
        {
            Game.ObjPool.ClearAll();
            Game.Map?.ClearMap();
            Game.Map = null;
            Game.Player = null;
        }

        public void BackToStartScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
        }

        public void BackToStartScene(int seconds)
        {
            StartCoroutine(WaitAndBack(seconds));
        }

        private IEnumerator WaitAndBack(int seconds)
        {
            yield return new WaitForSeconds(seconds);
            BackToStartScene();
        }

        abstract public void OnMapClicked(int posx, int posy);
        abstract public void ShowTips(params string[] texts);
    }

}

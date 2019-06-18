using System;
using UnityEngine;

namespace ArmyAnt.ViewUtil {
    /// <summary>
    /// 关于屏幕截图的相关工具
    /// </summary>
    public static class ScreenUtil {
        /// <summary>
        /// 接取整个屏幕并保存到指定路径
        /// </summary>
        /// <param name="filepath"> 要保存的路径 </param>
        public static void CaptureScreen(string filepath) {
            ScreenCapture.CaptureScreenshot(filepath);
        }

        /// <summary>
        /// 截取指定摄像机的全部内容, 返回其纹理. 
        /// <remarks> 必须在协程或屏幕摄像机的 OnPostRender 中调用 </remarks>
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static Texture2D CaptureCamera(Camera cam) {
            RenderTexture rt = new RenderTexture(Convert.ToInt32(cam.rect.width), Convert.ToInt32(cam.rect.height), 0);

            cam.targetTexture = rt;
            cam.Render();

            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D(Convert.ToInt32(cam.rect.width), Convert.ToInt32(cam.rect.height), TextureFormat.RGBA32, false);

            screenShot.ReadPixels(cam.rect, 0, 0);
            screenShot.Apply();

            cam.targetTexture = null;
            RenderTexture.active = null;
            UnityEngine.Object.Destroy(rt);

            return screenShot;
        }

        /// <summary>
        /// 截取指定摄像机的全部内容, 返回其纹理. 
        /// <remarks> 必须在协程或屏幕摄像机的 OnPostRender 中调用 </remarks>
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Texture2D CaptureRect(Rect rect) {
            Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();
            return screenShot;
        }
    }
}

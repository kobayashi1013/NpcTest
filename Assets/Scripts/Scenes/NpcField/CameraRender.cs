using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Scenes.NpcField
{
    public class CameraRender : MonoBehaviour
    {
        [Header("Component")]
        [SerializeField] private Camera _camera;

        private void Start()
        {
            ScreenShot();
        }

        private void ScreenShot(int width = 1920, int height = 1080)
        {
            //レンダリング設定
            var renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            _camera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;

            //レンダリング
            _camera.Render();

            //テクスチャにコピー
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();

            //PNG保存
            byte[] bytes = texture.EncodeToPNG();
            string path = Path.Combine(Application.dataPath, "..", "test.png");
            File.WriteAllBytes(path, bytes);

            //レンダリング解放
            _camera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(renderTexture);
            Destroy(texture);

            Debug.Log("Image Path : " + path);
        }
    }
}

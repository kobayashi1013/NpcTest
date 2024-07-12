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
            //�����_�����O�ݒ�
            var renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            _camera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;

            //�����_�����O
            _camera.Render();

            //�e�N�X�`���ɃR�s�[
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();

            //PNG�ۑ�
            byte[] bytes = texture.EncodeToPNG();
            string path = Path.Combine(Application.dataPath, "..", "test.png");
            File.WriteAllBytes(path, bytes);

            //�����_�����O���
            _camera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(renderTexture);
            Destroy(texture);

            Debug.Log("Image Path : " + path);
        }
    }
}

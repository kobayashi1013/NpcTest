using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prefabs
{
    public class ViewPhotography : MonoBehaviour
    {
        [SerializeField] private Camera _viewCamera;

        public string ScreenShot(int width = 1920, int height = 1080)
        {
            //�����_�����O�ݒ�
            var renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            _viewCamera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;

            //�����_�����O
            _viewCamera.Render();

            //�e�N�X�`���ɃR�s�[
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();

            //PNG�f�[�^�ϊ�
            byte[] bytes = texture.EncodeToPNG();
            string pngData = System.Convert.ToBase64String(bytes);

            //�����_�����O���
            _viewCamera.targetTexture = null;
            RenderTexture.active = null;
            renderTexture.Release();
            Destroy(texture);

            return pngData;
        }
    }
}
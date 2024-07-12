using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Prefabs
{
    public class GptConnection : MonoBehaviour
    {
        [SerializeField] private ViewPhotography _viewPhotography;
        [SerializeField] private string _googleAppsScriptId; //Google App Script ID

        private string _googleAppsScriptUrl;

        private void Start()
        {
            _googleAppsScriptUrl = "https://script.google.com/macros/s/" + _googleAppsScriptId + "/exec";

            StartCoroutine(ApiConnection(_viewPhotography.ScreenShot()));
        }

        private IEnumerator ApiConnection(string imageEncode)
        {
            WWWForm form = new WWWForm();
            form.AddField("imageEncode", imageEncode);
            UnityWebRequest request = UnityWebRequest.Post(_googleAppsScriptUrl, form);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError
                || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("error : " + request.result);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
            }
        }
    }
}
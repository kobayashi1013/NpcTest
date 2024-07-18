using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Expansion;

namespace Prefabs
{
    public class GptConnection : MonoBehaviour
    {
        [SerializeField] private ViewPhotography _viewPhotography;
        [SerializeField] private string _googleAppsScriptId; //Google App Script ID
        [SerializeField] private string _prompt; //プロンプト文

        private string _googleAppsScriptUrl;

        //JSONからの返答を格納する
        [System.Serializable]
        public class GetResponseData
        {
            [System.Serializable]
            public class Choice
            {
                public Message message;
            }

            [System.Serializable]
            public class Message
            {
                public string content;
            }

            public Choice[] choices;
        }

        private void Awake()
        {
            _googleAppsScriptUrl = "https://script.google.com/macros/s/" + _googleAppsScriptId + "/exec";
        }

        private async void Start()
        {
            //var screenShot = _viewPhotography.ScreenShot();
            var response = await GetResponse("こんにちは。");
            Debug.Log(response);
        }

        public async Task<string> GetResponse(string input)
        {
            //APIキーの取得
            var apiKey = await GetApiKey(_googleAppsScriptUrl);
            if (apiKey == null) return "Error : Cannot Get APIKEY";

            //GPTとの接続
            var response = await ApiConnection(apiKey, _prompt, input);
            return response;
        }

        /// <summary>
        /// APIキーの取得
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<string> GetApiKey(string url)
        {
            //リクエスト作成
            UnityWebRequest request = UnityWebRequest.Get(url);

            //リクエスト送信
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = request.downloadHandler.text;
                return response;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// APIとの接続
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="prompt"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<string> ApiConnection(string apiKey, string prompt, string input)
        {
            //APIのURL
            var url = "https://api.openai.com/v1/chat/completions";

            //リクエストの設定
            var payload = new
            {
                model = "gpt-4o",
                messages = new[]
                {
                    new { role = "system", content = prompt },
                },
                max_tokens = 100,
                temperature = 1
            };

            //Jsonの作成
            string jsonData = JsonConvert.SerializeObject(payload);

            //リクエストの作成
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            //リクエスト送信
            await request.SendWebRequest();

            //返答
            if (request.result == UnityWebRequest.Result.ConnectionError
                || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.downloadHandler.text);
                return "Error : " + request.error;
            }

            var responseText = request.downloadHandler.text;
            var response = JsonConvert.DeserializeObject<GetResponseData>(responseText);

            if (response != null && response.choices != null && response.choices.Length > 0)
            {
                return response.choices[0].message.content;
            }
            else
            {
                return "Error : GPT Response is null";
            }
        }
    }
}
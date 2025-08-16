using System.Net;
using System;
using System.IO;
using System.Text;
//using System.Collections.Generic;

using System.Collections;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

using DFCommonLib.Logger;
using DFCommonLib.Utils;

namespace DFCommonLib.HttpApi
{
    public interface IDFRestClient
    {
        void SetEndpoint(string endpoint);
        Task<WebAPIData> Ping();
    }

    public class DFRestClient : IDFRestClient
    {
        private const string UserAgentName = "DFRestClient";

        private static readonly HttpClient client = new HttpClient();
        protected IDFLogger<DFRestClient> _logger;
        protected string _endpoint;

        private string _accessToken;
        private DateTime _tokenExpiry;

        public DFRestClient(IDFLogger<DFRestClient> logger)
        {
            _logger = logger;
        }

        public DFRestClient()
        {
            _logger = DFServices.GetService<IDFLogger<DFRestClient>>();
        }

        virtual protected string GetHostname()
        {
            if (_endpoint != null)
            {
                return _endpoint;
            }
            return "http://NO_ENDPOINT_SET";
        }

        virtual protected string GetModule()
        {
            return null;
        }

        public void SetEndpoint(string endpoint)
        {
            _endpoint = endpoint;
        }

        public string GetEndpoint()
        {
            return _endpoint;
        }

        public static string EncodeInput(string plaintext)
        {
            var data = Encoding.UTF8.GetBytes(plaintext);
            return Convert.ToBase64String(data);
        }

        public static string DecodeInput(string encodedString)
        {
            var data = Convert.FromBase64String(encodedString);
            string decodedString = Encoding.UTF8.GetString(data);
            return decodedString;
        }

        private string GetFullUrl(string method)
        {
            var hostname = GetHostname();
            if (hostname != null)
            {
                var module = GetModule();

                if (module != null)
                {
                    return String.Format("{0}/{1}/{2}", hostname, module, method);
                }
                return String.Format("{0}/{1}", hostname, method);
            }
            return null;
        }

        private T ConvertFromRestData<T>(WebAPIData apiData) where T : WebAPIData, new()
        {
            if (apiData.errorCode == 0)
            {
                var data = JsonConvert.DeserializeObject<T>(apiData.message);
                return data;
            }
            var cls = new T();
            cls.errorCode = apiData.errorCode;
            cls.message = apiData.message;
            return cls;
        }

        public async Task<WebAPIData> Ping()
        {
            var data = await GetJsonData(0, "Ping");
            if (data == null)
            {
                _logger.LogWarning("DFRestClient: Ping returned null data");
                return new WebAPIData { errorCode = 500, message = "Ping returned null data" };
            }
            return new WebAPIData { errorCode = data.errorCode, message = data.message };
        }

        public async Task<WebAPIData> GetJsonData(int methodId, string url)
        {

            var fullUrl = GetFullUrl(url);
            var webRequest = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            webRequest.Headers.Add("User-Agent", UserAgentName);
            if (!string.IsNullOrEmpty(_accessToken))
            {
                webRequest.Headers.Add("Authorization", "Bearer " + _accessToken);
            }

            var data = await HandleRequest(methodId, webRequest);
            return data;
        }

        public async Task<T> GetJsonData<T>(int methodId, string url) where T : WebAPIData, new()
        {
            var data = await GetJsonData(methodId, url);
            var result = ConvertFromRestData<T>(data);
            return result;
        }

        public async Task<WebAPIData> PostJsonData(int methodId, string url, string jsonData)
        {
            var fullUrl = GetFullUrl(url);
            var webRequest = new HttpRequestMessage(HttpMethod.Post, fullUrl);
            webRequest.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            webRequest.Headers.Add("User-Agent", UserAgentName);
            if (!string.IsNullOrEmpty(_accessToken))
            {
                webRequest.Headers.Add("Authorization", "Bearer " + _accessToken);
            }

            var data = await HandleRequest(methodId, webRequest);
            return data;
        }

        public async Task<WebAPIData> PostJsonData(int methodId, string postUrl, object obj)
        {
            string jsonData = await Task.Run(() => JsonConvert.SerializeObject(obj));
            var data = await PostJsonData(methodId, postUrl, jsonData);
            return data;
        }

        public async Task<WebAPIData> PutJsonData(int methodId, string url, string jsonData)
        {
            var fullUrl = GetFullUrl(url);
            var webRequest = new HttpRequestMessage(HttpMethod.Put, fullUrl);
            webRequest.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            webRequest.Headers.Add("User-Agent", UserAgentName);
            if (!string.IsNullOrEmpty(_accessToken))
            {
                webRequest.Headers.Add("Authorization", "Bearer " + _accessToken);
            }
            var data = await HandleRequest(methodId, webRequest);
            return data;
        }

        public async Task<T> PutJsonDataAs<T>(int methodId, string url, string jsonData) where T : WebAPIData, new()
        {
            var data = await PutJsonData(methodId, url, jsonData);
            var result = ConvertFromRestData<T>(data);
            return result;
        }


        public async Task<WebAPIData> PutData(int methodId, string postUrl, object obj)
        {
            string jsonData = await Task.Run(() => JsonConvert.SerializeObject(obj));
            var data = await PutJsonData(methodId, postUrl, jsonData);
            return data;
        }

        public async Task<T> PutDataAs<T>(int methodId, string postUrl, object obj) where T : WebAPIData, new()
        {
            string jsonData = await Task.Run(() => JsonConvert.SerializeObject(obj));
            var data = await PutJsonDataAs<T>(methodId, postUrl, jsonData);
            return data;
        }


        private string GetUrl(HttpRequestMessage webRequest)
        {
            var uri = webRequest.RequestUri;
            if (uri != null)
            {
                return uri.AbsoluteUri;
            }
            return "";
        }

        protected virtual async Task<WebAPIData> HandleRequest(int methodId, HttpRequestMessage webRequest)
        {
            var webUrl = GetUrl(webRequest);

            try
            {
                var returnData = await client.SendAsync(webRequest);
                if (returnData.IsSuccessStatusCode)
                {
                    if (returnData.Content != null)
                    {
                        string dataString = await Task.Run(() => returnData.Content.ReadAsStringAsync());
                        return new WebAPIData(0, dataString);
                    }
                    else
                    {
                        _logger.LogWarning(string.Format("DFRestClient: {0} => 500:Could not read content ", webUrl));
                        return new WebAPIData(500, "Could not read content");
                    }
                }

                _logger.LogWarning(string.Format("DFRestClient: {0} => {1}:{2} ",
                    webUrl,
                    returnData.StatusCode,
                    returnData.ReasonPhrase));

                return new WebAPIData((int)returnData.StatusCode, returnData.ReasonPhrase);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(string.Format("DFRestClient: {0} => {1}:{2} ",
                    webUrl,
                    500,
                    ex.ToString()));

                return new WebAPIData(500, ex.ToString());
            }
        }
    }
}


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

namespace DFCommonLib.HttpApi
{
    public interface IDFRestClient
    {
        void SetEndpoint(string endpoint);
        string PingServer();
    }

    public class DFRestClient : IDFRestClient
    {
        private static readonly HttpClient client = new HttpClient();
        protected IDFLogger<DFRestClient> _logger;
        protected string _endpoint;

        public DFRestClient(IDFLogger<DFRestClient> logger)
        {
            _logger = logger;
        }

        virtual protected string GetHostname()
        {
            if ( _endpoint != null )
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

        public string PingServer()
        {
            // Force async execution to wait for the result
            var data = GetJsonData(0, "PingServer").Result;
            if (data == null)
            {
                _logger.LogWarning("DFRestClient: PingServer returned null data");
                return "PingServer returned null data";
            }

            if (data.errorCode > 299)
            {
                var msg = string.Format("DFRestClient: PingServer failed with error code {0} and message: {1}",
                    data.errorCode,
                    data.message);
                _logger.LogWarning(msg);
                return msg;
            }

            if (data.message == null)
            {
                _logger.LogWarning("DFRestClient: PingServer returned null message");
                return "PingServer returned null message";
            }

            return data.message;
        }

        private string GetFullUrl(string method)
        {
            var hostname = GetHostname();
            if ( hostname != null )
            {
                var module = GetModule();

                if ( module != null )
                {
                    return String.Format("{0}/{1}/{2}", hostname, module, method);
                }
                return String.Format("{0}/{1}", hostname, method);
            }
            return null;
        }

        private WebAPIData ConvertFromRestData<T>(WebAPIData apiData) where T:WebAPIData, new()
        {
            if ( apiData.errorCode == 0 )
            {
                var data = JsonConvert.DeserializeObject<T>(apiData.message);
                return data;
            }
            var cls = new T();
            cls.errorCode = apiData.errorCode;
            cls.message = apiData.message;
            return cls;
        }

        public async Task<WebAPIData> GetJsonData(int methodId, string url)
        {
            var fullUrl = GetFullUrl(url);
            var webRequest = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            //webRequest.Headers.Add("Content-Type", "application/json");
            webRequest.Headers.Add("User-Agent", "DarkFactor BE");

            //Debug.LogWarning("Get:" + fullUrl);

            var data = await HandleRequest(methodId, webRequest, _logger);
            return data;
        }

        public async Task<WebAPIData> GetJsonData<T>(int methodId, string url) where T:WebAPIData, new()
        {
            var data = await GetJsonData(methodId, url);
            var result = ConvertFromRestData<T>(data);
            return result;
        }

        public async Task<WebAPIData> PostJsonData(int methodId,string url, string jsonData)
        {
            var fullUrl = GetFullUrl(url);
            var webRequest = new HttpRequestMessage(HttpMethod.Post, fullUrl);
            webRequest.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var data = await HandleRequest(methodId, webRequest, _logger);
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
            var webRequest = new HttpRequestMessage(HttpMethod.Put , fullUrl);
            webRequest.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var data = await HandleRequest(methodId, webRequest, _logger);
            return data;
        }

        public async Task<WebAPIData> PutJsonData(int methodId, string postUrl, object obj)
        {
            string jsonData = await Task.Run(() => JsonConvert.SerializeObject(obj));
            var data = await PutJsonData(methodId, postUrl, jsonData);
            return data;
        }

        private static string GetUrl( HttpRequestMessage webRequest )
        {
            var uri = webRequest.RequestUri;
            if ( uri != null )
            {
                return uri.AbsoluteUri;
            }
            return "";
        }

        private static async Task<WebAPIData> HandleRequest(int methodId, HttpRequestMessage webRequest, IDFLogger<DFRestClient> logger)
        {
            var webUrl = GetUrl( webRequest );

            try
            {
                var returnData = await client.SendAsync( webRequest );
                if ( returnData.IsSuccessStatusCode )
                {
                    if ( returnData.Content != null )
                    {
                        string dataString = await Task.Run(() => returnData.Content.ReadAsStringAsync() );
                        return new WebAPIData( 0, dataString );
                    }
                    else
                    {
                        logger.LogWarning( string.Format("DFRestClient: {0} => 500:Could not read content ", webUrl));
                        return new WebAPIData( 500, "Could not read content" );
                    }
                }

                logger.LogWarning( string.Format("DFRestClient: {0} => {1}:{2} ", 
                    webUrl, 
                    returnData.StatusCode,
                    returnData.ReasonPhrase ));

                return new WebAPIData( (int) returnData.StatusCode , returnData.ReasonPhrase );
            }
            catch( Exception ex )
            {
                logger.LogWarning( string.Format("DFRestClient: {0} => {1}:{2} ", 
                    webUrl, 
                    500,
                    ex.ToString() ));

                return new WebAPIData( 500 , ex.ToString() );
            }
        }
    }
}
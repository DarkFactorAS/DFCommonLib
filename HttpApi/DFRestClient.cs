

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
    public class WebAPIData
    {
        public int errorCode;
        public string message;

        public WebAPIData(int errorCode, string message )
        {
            this.errorCode = errorCode;
            this.message = message;
        }
    }

    public class DFRestClient
    {
        private static readonly HttpClient client = new HttpClient();
        protected IDFLogger<DFRestClient> _logger;

        public DFRestClient(IDFLogger<DFRestClient> logger)
        {
            _logger = logger;
        }

        virtual protected string GetHostname()
        {
            return "http://127.0.0.1";
        }

        virtual protected string GetModule()
        {
            return null;
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

        public async Task<WebAPIData> GetJsonData(int methodId, string url)
        {
            var fullUrl = GetFullUrl(url);
            var webRequest = new HttpRequestMessage(HttpMethod.Get, url);
            webRequest.Headers.Add("Content-Type", "application/json");
            webRequest.Headers.Add("User-Agent", "DarkFactor BE");

            //Debug.LogWarning("Get:" + fullUrl);

            var data = await HandleRequest(methodId, webRequest, _logger);
            return data;
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

        public async Task<WebAPIData> PutJsonData(int methodId,string url, string jsonData)
        {
            var fullUrl = GetFullUrl(url);
            var webRequest = new HttpRequestMessage(HttpMethod.Put , fullUrl);
            webRequest.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");;
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
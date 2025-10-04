

using System.Text;
using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using DFCommonLib.HttpApi;
using DFCommonLib.Logger;
using Newtonsoft.Json;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using DFCommonLib.Utils;

namespace DFCommonLib.HttpApi.OAuth2
{
    public interface IDFOAuth2RestClient : IDFHttpRestClient
    {
        void SetAuthClient(OAuth2ClientData clientData);
        Task<string> AuthenticateIfNeeded();
    }

    public class DFOAuth2RestClient : DFHttpRestClient, IDFOAuth2RestClient
    {
        private const int POST_AUTH = 1;
        private const int POST_CODE = 2;

        private DateTime _authExpiryTime = DateTime.MinValue;

        private OAuth2ClientData _clientData;

        public DFOAuth2RestClient() : base()
        {
            _accessToken = null;
            _clientData = null;
        }

        override protected string GetModule()
        {
            return "ServerOAuth2";
        }

        public void SetAuthClient(OAuth2ClientData clientData)
        {
            _clientData = clientData;
            if ( _clientData != null)
            {
                _clientData.State = Guid.NewGuid().ToString();
            }
        }

        public async Task<string> AuthenticateIfNeeded()
        {
            if (_clientData == null)
            {
                _logger.LogError("OAuth2 client data is not set.");
                return null;
            }
            
            if (_clientData.IsValid() == false)
            {
                _logger.LogError("OAuth2 client data is not valid.");
                return null;
            }

            if (!string.IsNullOrEmpty(_accessToken) && _authExpiryTime > DateTime.UtcNow)
            {
                return _accessToken;
            }

            var response = await LoginOAuth2Client(_clientData);
            if (response == null || response.errorCode != 0 || response.State != _clientData.State)
            {
                _logger.LogError("OAuth2 login failed. Client: {0}", _clientData.ToString());
                return null;
            }

            // Trade code for access token
            var oauth2CodeData = new OAuth2CodeData
            {
                Code = response.Code,
                State = response.State
            };

            OAuth2CodeResponse codeResult = await LoginOAuth2WithCode(oauth2CodeData);
            if (codeResult == null || codeResult.errorCode != 0 || codeResult.State != response.State)
            {
                _logger.LogError("OAuth2 code exchange failed. Code: {0}, State: {1}", oauth2CodeData.Code, oauth2CodeData.State);
                return null;
            }

            _accessToken = codeResult.AccessToken;
            _authExpiryTime = DateTime.UtcNow.AddSeconds(codeResult.ExpiresIn);
            return _accessToken;
        }

        private async Task<OAuth2AuthResponse> LoginOAuth2Client(OAuth2ClientData oauth2ClientData)
        {
            return await base.PutDataAs<OAuth2AuthResponse>(POST_AUTH, "Auth", oauth2ClientData);
        }

        private async Task<OAuth2CodeResponse> LoginOAuth2WithCode(OAuth2CodeData codeData)
        {
            return await base.PutDataAs<OAuth2CodeResponse>(POST_CODE, "Code", codeData);
        }

        public static void SetupService(IServiceCollection services)
        {
            services.AddTransient<IDFOAuth2RestClient, DFOAuth2RestClient>();
        }
    }
}
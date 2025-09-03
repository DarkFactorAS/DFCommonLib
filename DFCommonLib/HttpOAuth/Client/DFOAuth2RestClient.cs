

using System.Text;
using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using DFCommonLib.HttpApi;
using DFCommonLib.Logger;
using Newtonsoft.Json;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace DFCommonLib.HttpApi.OAuth2
{
    public interface IDFOAuth2RestClient : IDFRestClient
    {
        void SetAuthClient(OAuth2ClientData clientData);
        Task<string> AuthenticateIfNeeded();
    }

    public class DFOAuth2RestClient : DFRestClient, IDFOAuth2RestClient
    {
        private const int POST_AUTH = 1;
        private const int POST_CODE = 2;

        private OAuth2ClientData _clientData;

        public DFOAuth2RestClient() : base()
        {
        }

        override protected string GetModule()
        {
            return "ServerOAuth2";
        }

        public void SetAuthClient(OAuth2ClientData clientData)
        {
            _clientData = clientData;
            _clientData.State = Guid.NewGuid().ToString();
        }

        public async Task<string> AuthenticateIfNeeded()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                var response = await LoginOAuth2Client(_clientData);
                if (response == null || response.errorCode != 0 || response.State != _clientData.State)
                {
                    throw new Exception("Authentication failed");
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
                    throw new Exception("Authentication failed");
                }

                _accessToken = codeResult.AccessToken;
            }
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
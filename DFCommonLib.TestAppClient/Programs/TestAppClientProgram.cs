

using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using DFCommonLib.Config;
using DFCommonLib.HttpApi.OAuth2;
using DFCommonLib.Logger;
using DFCommonLib.Utils;
using TestApp.Model;

namespace TestApp
{
    public class TestAppClientProgram
    {
        private IDFLogger<TestAppClientProgram> _logger;
        private ITestRestClient _commonRestClient;
        private ITestAuthRestClient _commonAuthRestClient;
        private OAuth2ClientData _oauth2ClientData;

        public TestAppClientProgram(IConfigurationHelper configurationHelper)
        {
            _logger = DFServices.GetService<IDFLogger<TestAppClientProgram>>();
            _commonRestClient = new TestRestClient();
            _commonAuthRestClient = new TestAuthRestClient();

            // Set the API endpoint and key from the configuration
            var config = configurationHelper.Settings as TestAppConfig;
            if (config != null)
            {
                var commonLibServer = config.CommonLibServer;
                if (commonLibServer == null || string.IsNullOrEmpty(commonLibServer.Endpoint) ||
                    string.IsNullOrEmpty(commonLibServer.ClientId) || string.IsNullOrEmpty(commonLibServer.ClientSecret))
                {
                    _logger.LogError("CommonLibServer configuration is missing or incomplete.");
                    throw new Exception("CommonLibServer configuration is missing or incomplete.");
                }

                _oauth2ClientData = new OAuth2ClientData
                {
                    ClientId = commonLibServer.ClientId,
                    ClientSecret = commonLibServer.ClientSecret,
                    Scope = commonLibServer.Scope
                };

                _commonRestClient.SetEndpoint(commonLibServer.Endpoint);
                _commonAuthRestClient.SetEndpoint(commonLibServer.Endpoint);
                _commonAuthRestClient.SetAuthClient(null);

                var msg = string.Format("Connecting to API : {0}", commonLibServer.Endpoint);
                _logger.LogInfo(msg);
            }
        }

        private async Task<bool> RunPing()
        {
            var pingResult = await _commonRestClient.Ping();
            if (pingResult.message == "PONG")
            {
                _logger.LogInfo($"Ping succeeded: {pingResult.message}");
                return true;
            }
            _logger.LogError($"Ping failed: {pingResult.message}");
            return false;
        }

        private async Task<bool> RunAuthPing()
        {
            var pingResult = await _commonAuthRestClient.Ping();
            if (pingResult.message == "PONG")
            {
                _logger.LogInfo($"Ping succeeded: {pingResult.message}");
                return true;
            }
            _logger.LogError($"Ping failed: {pingResult.message}");
            return false;
        }

        private async Task<bool> RunAuthenticateIfNeeded_NoClientData()
        {
            _commonAuthRestClient.SetAuthClient(null);
            var authResult = await _commonAuthRestClient.AuthenticateIfNeeded();
            if (string.IsNullOrEmpty(authResult))
            {
                _logger.LogInfo($"RunAuthenticateIfNeeded_NoClientData succeeded");
                return true;
            }
            _logger.LogError($"RunAuthenticateIfNeeded_NoClientData failed: {authResult}");
            return false;
        }

        private async Task<bool> RunAuthenticateIfNeeded_BrokenClientData()
        {
            _commonAuthRestClient.SetAuthClient(new OAuth2ClientData
            {
                ClientId = "broken_clientid",
                ClientSecret = "broken_clientsecret",
                Scope = "broken_scope"
            });
            var authResult = await _commonAuthRestClient.AuthenticateIfNeeded();
            if (string.IsNullOrEmpty(authResult))
            {
                _logger.LogInfo($"RunAuthenticateIfNeeded_BrokenClientData succeeded");
                return true;
            }
            _logger.LogError($"RunAuthenticateIfNeeded_BrokenClientData failed: {authResult}");
            return false;
        }

        private async Task<bool> RunAuthenticateIfNeeded_OKClientData()
        {
            _commonAuthRestClient.SetAuthClient(_oauth2ClientData);
            var authResult = await _commonAuthRestClient.AuthenticateIfNeeded();
            if (!string.IsNullOrEmpty(authResult))
            {
                _logger.LogInfo($"RunAuthenticateIfNeeded_OKClientData succeeded");
                return true;
            }
            _logger.LogError($"RunAuthenticateIfNeeded_OKClientData failed: {authResult}");
            return false;
        }

        public async Task<bool> Run()
        {
            if (!await RunPing())
            {
                return false;
            }
            if (!await RunAuthPing())
            {
                return false;
            }
            if (!await RunAuthenticateIfNeeded_NoClientData())
            {
                return false;
            }
            if (!await RunAuthenticateIfNeeded_BrokenClientData())
            {
                return false;
            }
            if (!await RunAuthenticateIfNeeded_OKClientData())
            {
                return false;
            }
            return true;
        }
    }
}
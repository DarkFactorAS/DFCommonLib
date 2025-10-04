

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
        private ITestRestClient _commonRestClient;
        private ITestAuthRestClient _commonAuthRestClient;

        public TestAppClientProgram(IConfigurationHelper configurationHelper)
        {
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
                    DFLogger.LogOutput(DFLogLevel.ERROR, "CommonLibServer", "CommonLibServer configuration is missing or incomplete.");
                    throw new Exception("CommonLibServer configuration is missing or incomplete.");
                }

                _commonRestClient.SetEndpoint(commonLibServer.Endpoint);
                _commonAuthRestClient.SetEndpoint(commonLibServer.Endpoint);
                _commonAuthRestClient.SetAuthClient(new OAuth2ClientData
                {
                    ClientId = commonLibServer.ClientId,
                    ClientSecret = commonLibServer.ClientSecret,
                    Scope = commonLibServer.Scope
                });

                var msg = string.Format("Connecting to API : {0}", commonLibServer.Endpoint);
                DFLogger.LogOutput(DFLogLevel.INFO, "CommonLibClient", msg);
            }
        }

        private async Task<bool> RunPing()
        {
            var pingResult = await _commonRestClient.Ping();
            if (pingResult.message == "PONG")
            {
                DFLogger.LogOutput(DFLogLevel.INFO, "CommonLibClientProgram", $"Ping succeeded: {pingResult.message}");
                return true;
            }
            DFLogger.LogOutput(DFLogLevel.ERROR, "CommonLibClientProgram", $"Ping failed: {pingResult.message}");
            return false;
        }

        private async Task<bool> RunAuthPing()
        {
            var pingResult = await _commonAuthRestClient.Ping();
            if (pingResult.message == "PONG")
            {
                DFLogger.LogOutput(DFLogLevel.INFO, "CommonLibClientProgram", $"Ping succeeded: {pingResult.message}");
                return true;
            }
            DFLogger.LogOutput(DFLogLevel.ERROR, "CommonLibClientProgram", $"Ping failed: {pingResult.message}");
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
            return true;
        }
    }
}
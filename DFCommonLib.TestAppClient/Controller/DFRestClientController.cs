using Microsoft.AspNetCore.Mvc;
using DFCommonLib.HttpApi;
using DFCommonLib.Logger;
using DFCommonLib.Utils;
using DFCommonLib.Config;
using TestApp;
using TestApp.Model;
using System.Threading.Tasks;
using DFCommonLib.HttpApi.OAuth2;

namespace DFCommonLib.TestApp.Controller
{

    public class DFRestClientController : DFHttpRestClient
    {
        private readonly TestRestClient _restClient;
        private readonly TestAuthRestClient _authRestClient;

        public DFRestClientController()
        {
            var configurationHelper = DFServices.GetService<IConfigurationHelper>();
            var config = configurationHelper.Settings as TestAppConfig;

            _restClient = new TestRestClient();
            _restClient.SetEndpoint(config.TestApi.Endpoint);

            _authRestClient = new TestAuthRestClient();
            _authRestClient.SetEndpoint(config.TestApi.Endpoint);
            _authRestClient.SetAuthClient(new OAuth2ClientData
            {
                ClientId = config.AppName,
                ClientSecret = config.TestApi.ApiKey
            });
        }

        [HttpGet("RunAllPrograms")]
        public async Task<string> RunAllPrograms()
        {
            RunLogTest();
            RunPingTest();
            await RunModelClassTest();
            await RunModelNoAuthClassTest();
            await RunModelAuthClassTest();
            return "All programs are running";
        }

        [HttpGet("RunLogTest")]
        public void RunLogTest()
        {
            _logger.Startup("Log Test", "9.9.9");
            _logger.LogInfo("This is an info message.");
            _logger.LogInfo("This is an info message with args. {0}:{1}", "arg1", "arg2");
            _logger.LogDebug("This is a debug message. {0}:{1}", "arg1", "arg2");
            _logger.LogImportant("This is an important message. {0}:{1}", "arg1", "arg2");
            _logger.LogWarning("This is a warning message. {0}:{1}", "arg1", "arg2");
            _logger.LogError("This is an error message. {0}:{1}", "arg1", "arg2");
            _logger.LogException("This is an exception message", new System.Exception("Sample exception"));
            _logger.LogException("This is an exception message {0}:{1}", new System.Exception("Sample exception"), "arg1", "arg2");
        }

        [HttpGet("RunPingTest")]
        public string RunPingTest()
        {
            var result = _restClient.Ping().Result;
            if (result == null || result.errorCode != 0 || string.IsNullOrEmpty(result.message))
            {
                throw new System.Exception($"Ping failed");
            }
            _logger.LogDebug($"Ping result: {result.message}");
            if (result.message != "PONG")
            {
                throw new System.Exception($"Ping failed with unexpected message: {result.message}");
            }
            return result.message;
        }

        [HttpGet("RunModelClassTest")]
        public async Task<RestDataModel> RunModelClassTest()
        {
            var model = new RestDataModel
            {
                Id = 1,
                Name = "Test Model"
            };
            RestDataModel resultModel = await _restClient.TestModelClass(model);
            _logger.LogDebug($"Model class test result: {resultModel.Name}");
            return resultModel;
        }


        //
        // Try to call the method that requires authorization without token
        //
        [HttpGet("RunModelNoAuthClassTest")]
        public async Task<RestDataModel> RunModelNoAuthClassTest()
        {
            var model = new RestDataModel
            {
                Id = 1,
                Name = "Test NoAuth Model"
            };
            RestDataModel resultModel = await _restClient.TestAuthModelClass(model);
            _logger.LogDebug($"Model class test result: {resultModel.Name}");
            return resultModel;
        }

        //
        // Try to call the method that requires authorization with automatically fetch token
        //
        [HttpGet("RunModelAuthClassTest")]
        public async Task<RestDataModel> RunModelAuthClassTest()
        {
            var model = new RestDataModel
            {
                Id = 1,
                Name = "Test Auth Model"
            };
            RestDataModel resultModel = await _authRestClient.TestAuthModelClass(model);
            _logger.LogDebug($"Model class test result: {resultModel.Name}");
            return resultModel;
        }
    }
}

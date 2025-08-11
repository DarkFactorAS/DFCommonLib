using Microsoft.AspNetCore.Mvc;
using DFCommonLib.HttpApi;
using DFCommonLib.Logger;
using DFCommonLib.Utils;
using DFCommonLib.Config;
using TestApp;
using TestApp.Model;
using System.Threading.Tasks;

namespace DFCommonLib.TestApp.Controller
{
    
    public class DFRestClientController : DFRestClient
    {
        private readonly TestRestClient _restClient;

        public DFRestClientController()
        {
            var configurationHelper = DFServices.GetService<IConfigurationHelper>();
            var config = configurationHelper.Settings as TestAppConfig;

            _restClient = new TestRestClient();
            _restClient.SetEndpoint(config.TestApi.Endpoint);
        }

        [HttpGet("RunAllPrograms")]
        public async Task<string> RunAllPrograms()
        {
            RunLogTest();
            RunPingTest();
            await RunModelClassTest();
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
        public void RunPingTest()
        {
            // The test is set to ping the account server for now
            var result = _restClient.PingServer();
            _logger.LogDebug($"Ping result: {result}");
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
    }
}

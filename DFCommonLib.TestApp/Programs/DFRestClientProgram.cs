using System.Threading.Tasks;
using DFCommonLib.Config;
using DFCommonLib.HttpApi;
using DFCommonLib.Logger;
using DFCommonLib.Utils;
using TestApp;

namespace DFCommonLib.TestApp.Programs
{
    public class DFRestClientProgram
    {
        private readonly IDFRestClient _dfRestClient;

        public DFRestClientProgram()
        {
            var logger = DFServices.GetService<IDFLogger<DFRestClient>>();
            var configurationHelper = DFServices.GetService<IConfigurationHelper>();
            var config = configurationHelper.Settings as TestAppConfig;

            _dfRestClient = new DFRestClient(logger);
            _dfRestClient.SetEndpoint(config.TestApi.Endpoint);

            // The test is set to ping the account server for now
            var result = _dfRestClient.PingServer();
            logger.LogDebug($"Ping result: {result}");
        }
    }
}

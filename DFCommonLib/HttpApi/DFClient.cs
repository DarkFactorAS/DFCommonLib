
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DFCommonLib.HttpApi;
using Newtonsoft.Json;

namespace DFCommonLib.HttpApi
{
    public interface IDFClient
    {
        void SetEndpoint(string endpoint);
        string Ping();
        Task<WebAPIData> PingServer();
    }

    public class DFClient : IDFClient
    {
        private IDFRestClient _dfRestClient;

        public DFClient(IDFRestClient restClient)
        {
            _dfRestClient = restClient;
        }

        public void SetEndpoint(string endpoint)
        {
            _dfRestClient.SetEndpoint(endpoint);
        }

        public string Ping()
        {
            return _dfRestClient.PingServer();
        }

        public Task<WebAPIData> PingServer()
        {
            var ret = _dfRestClient.PingServer();
            if (ret == null)
            {
                return Task.FromResult(new WebAPIData
                {
                    errorCode = 500,
                    message = "Ping failed"
                });
            }

            return Task.FromResult(new WebAPIData
            {
                errorCode = 0,
                message = ret
            });
        }
    }
}
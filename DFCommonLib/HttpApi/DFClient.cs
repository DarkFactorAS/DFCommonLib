
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DFCommonLib.HttpApi;
using Newtonsoft.Json;

namespace DFCommonLib.HttpApi
{
    public interface IDFClient
    {
        void SetEndpoint(string endpoint);
        Task<WebAPIData> Ping();
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

        public async Task<WebAPIData> Ping()
        {
            return await _dfRestClient.Ping();
        }
    }
}
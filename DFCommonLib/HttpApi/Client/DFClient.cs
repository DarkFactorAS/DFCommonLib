
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
        private IDFHttpRestClient _dfRestClient;

        public DFClient(IDFHttpRestClient restClient)
        {
            _dfRestClient = restClient;
        }

        public void SetEndpoint(string endpoint)
        {
            _dfRestClient.SetEndpoint(endpoint);
        }

        public void ClearAccessToken()
        {
            _dfRestClient.ClearAccessToken();
        }

        public async Task<WebAPIData> Ping()
        {
            return await _dfRestClient.Ping();
        }

        public async Task<WebAPIData> Version()
        {
            return await _dfRestClient.Version();
        }
    }
}
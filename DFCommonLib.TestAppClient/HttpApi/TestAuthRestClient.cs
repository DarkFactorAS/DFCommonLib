

using System.Threading.Tasks;
using DFCommonLib.HttpApi;
using DFCommonLib.HttpApi.OAuth2;

namespace TestApp.Model
{
    public interface ITestAuthRestClient : IDFOAuth2RestClient
    {
        public Task<RestDataModel> TestAuthModelClass(RestDataModel model);
    }

    public class TestAuthRestClient : DFOAuth2RestClient, ITestAuthRestClient
    {
        private const int POST_TESTMODEL = 1;

        public TestAuthRestClient() : base()
        {
            // Initialize any specific properties or configurations for the test client here
        }

        public async Task<RestDataModel> TestAuthModelClass(RestDataModel model)
        {
            await AuthenticateIfNeeded();
            var response = await PutDataAs<RestDataModel>(POST_TESTMODEL, "TestAuthModelClass", model);
            return response;
        }

        protected override string GetModule()
        {
            // Override to provide a specific module name for testing purposes
            return "DFTestRestServer";
        }
    }
}
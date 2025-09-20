

using System.Threading.Tasks;
using DFCommonLib.HttpApi;

namespace TestApp.Model
{
    public interface ITestRestClient : IDFHttpRestClient
    {
        public Task<RestDataModel> TestModelClass(RestDataModel model);
    }

    public class TestRestClient : DFHttpRestClient, ITestRestClient
    {
        private const int POST_TESTMODEL = 1;

        public TestRestClient() : base()
        {
            // Initialize any specific properties or configurations for the test client here
        }

        public async Task<RestDataModel> TestModelClass(RestDataModel model)
        {
            var response = await PutDataAs<RestDataModel>(POST_TESTMODEL, "TestModelClass", model);
            return response;
        }

        public async Task<RestDataModel> TestAuthModelClass(RestDataModel model)
        {
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
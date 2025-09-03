

using DFCommonLib.Config;

namespace DFCommonLib.TestAppServer.Model
{
    public class TestApi
    {
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
    }

    public class TestAppConfig : AppSettings
    {
        public TestApi TestApi { get; set; }

        public TestAppConfig()
        {
        }
    }
}
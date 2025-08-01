

using DFCommonLib.Config;

namespace TestApp
{
    public class TestAppConfig : AppSettings
    {
        public class TestApi
        {
            public string Endpoint { get; set; }
            public string ApiKey { get; set; }
        }
    }
}
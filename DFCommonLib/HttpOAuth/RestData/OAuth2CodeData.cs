
// CS8632 fix: Enable nullable reference types
#nullable enable

namespace DFCommonLib.HttpApi.OAuth2
{
    public class OAuth2CodeData
    {
        public string? Code { get; set; }
        public string? State { get; set; }

        public OAuth2CodeData()
        {
        }

        public OAuth2CodeData(string code, string state)
        {
            Code = code;
            State = state;
        }
    }
}
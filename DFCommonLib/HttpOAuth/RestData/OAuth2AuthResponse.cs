// CS8632 fix: Enable nullable reference types
#nullable enable
using DFCommonLib.HttpApi;

namespace DFCommonLib.HttpApi.OAuth2
{
    public class OAuth2AuthResponse : WebAPIData
    {
        public string? Code { get; set; }
        public string? State { get; set; }
    }
}
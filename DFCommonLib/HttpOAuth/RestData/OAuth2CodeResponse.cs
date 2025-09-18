// CS8632 fix: Enable nullable reference types
#nullable enable
using DFCommonLib.HttpApi;

namespace DFCommonLib.HttpApi.OAuth2
{
    public class OAuth2CodeResponse : WebAPIData
    {
        public string? AccessToken { get; set; }
        public string? State { get; set; }
        public string? TokenType { get; set; }
        public uint ExpiresIn { get; set; }
    }
}
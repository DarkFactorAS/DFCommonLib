

using System.Collections;
using System.Collections.Generic;

namespace DFCommonLib.HttpApi.OAuth2
{
    public class OAuth2ClientModel
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public uint TokenExpiresInSeconds { get; set; }
        public string TokenIssuer { get; set; }
    }
}
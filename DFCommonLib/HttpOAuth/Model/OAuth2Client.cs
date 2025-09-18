

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFCommonLib.HttpApi.OAuth2
{
    public class OAuth2ClientModel
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
        [Required]
        public string Scope { get; set; }
        public uint TokenExpiresInSeconds { get; set; }
        [Required]
        public string TokenIssuer { get; set; }
    }
}
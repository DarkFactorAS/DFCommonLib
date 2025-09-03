


using DFCommonLib.Utils;
using Microsoft.AspNetCore.Mvc;

namespace DFCommonLib.HttpApi.OAuth2
{
    [ApiController]
    [Route("[controller]")]

    public class DFRestOAuth2ServerController : DFRestServerController
    {
        IServerOAuth2Provider _provider;

        public DFRestOAuth2ServerController(IServerOAuth2Provider provider)
        {
            _provider = provider;
        }

        public DFRestOAuth2ServerController()
        {
            _provider = DFServices.GetService<IServerOAuth2Provider>();
        }

        [HttpPut]
        [Route("auth")]
        public OAuth2AuthResponse Auth(OAuth2ClientData clientData)
        {
            return _provider.Auth(clientData);
        }

        [HttpPut]
        [Route("code")]
        public OAuth2CodeResponse Code(OAuth2CodeData codeData)
        {
            return _provider.Code(codeData);
        }
    }
}
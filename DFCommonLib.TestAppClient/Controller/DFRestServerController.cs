using Microsoft.AspNetCore.Mvc;
using DFCommonLib.HttpApi;
using DFCommonLib.Logger;
using TestApp.Model;
using Microsoft.AspNetCore.Authorization;
using DFCommonLib.HttpApi.OAuth2;

namespace DFCommonLib.TestApp.Controller
{
    public class DFTestRestServerController : DFRestOAuth2ServerController
    {
        public DFTestRestServerController() : base()
        {
        }

        [HttpPut("TestModelClass")]
        public RestDataModel TestModelClass(RestDataModel model)
        {
            return new RestDataModel
            {
                Id = model.Id,
                Name = model.Name
            };
        }

        [Authorize]
        [HttpPut("TestAuthModelClass")]
        public RestDataModel TestAuthModelClass(RestDataModel model)
        {
            return new RestDataModel
            {
                Id = model.Id,
                Name = model.Name
            };
        }

    }
}

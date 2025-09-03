using Microsoft.AspNetCore.Mvc;
using DFCommonLib.HttpApi;
using DFCommonLib.Logger;
using DFCommonLib.TestAppServer.Model;
using Microsoft.AspNetCore.Authorization;
using DFCommonLib.HttpApi.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DFCommonLib.TestAppServer.Controller
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
                Name = "Server:" + model.Name
            };
        }

        [Authorize (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("TestAuthModelClass")]
        public RestDataModel TestAuthModelClass(RestDataModel model)
        {
            return new RestDataModel
            {
                Id = model.Id,
                Name = "Server Auth OK:" + model.Name
            };
        }

    }
}

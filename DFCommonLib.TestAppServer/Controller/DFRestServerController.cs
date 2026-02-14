using System;
using Microsoft.AspNetCore.Mvc;
using DFCommonLib.HttpApi;
using DFCommonLib.Logger;
using DFCommonLib.TestAppServer.Model;
using Microsoft.AspNetCore.Authorization;
using DFCommonLib.HttpApi.OAuth2;
using DFCommonLib.HttpApi.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Linq;

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

        [Authorize(AuthenticationSchemes = OAuth2Static.AuthenticationScheme)]
        [RequireScope("write")]
        [HttpPut("TestAuthModelClass")]
        public RestDataModel TestAuthModelClass(RestDataModel model)
        {
            return new RestDataModel
            {
                Id = model.Id,
                Name = "Server Auth OK with Write Scope:" + model.Name
            };
        }

        public override string Version()
        {
            return Program.AppVersion;
        }
    }
}

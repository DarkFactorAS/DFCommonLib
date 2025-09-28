using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DFCommonLib.HttpApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using DFCommonLib.Utils;

namespace DFCommonLib.HttpApi
{
    [ApiController]
    [Route("[controller]")]
    public class DFRestServerController : ControllerBase
    {
        private readonly ILogger<DFRestServerController> _logger;

        public DFRestServerController()
        {
            _logger = DFServices.GetService<ILogger<DFRestServerController>>();
        }

        [AllowAnonymous]
        [HttpGet("Ping")]
        public virtual string Ping()
        {
            _logger.LogInformation("Ping received at {Time}", DateTime.UtcNow);
            return "PONG";
        }

        [AllowAnonymous]
        [HttpGet("Version")]
        public virtual string Version()
        {
            _logger.LogInformation("Version requested at {Time}", DateTime.UtcNow);
            return "0.0.0";
        }
    }
}

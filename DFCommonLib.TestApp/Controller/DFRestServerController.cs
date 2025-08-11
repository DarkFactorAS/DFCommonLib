using Microsoft.AspNetCore.Mvc;
using DFCommonLib.HttpApi;
using DFCommonLib.Logger;
using TestApp.Model;

namespace DFCommonLib.TestApp.Controller
{
    public class DFTestRestServerController : DFRestServerController
    {
        public DFTestRestServerController() : base()
        {
            //            ILogger<DFTestRestServerController> logger
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
    }
}

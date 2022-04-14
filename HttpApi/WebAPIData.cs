
using System;

namespace DFCommonLib.HttpApi
{
    public class WebAPIData
    {
        public int errorCode;
        public string message;

        public WebAPIData()
        {
        }

        public WebAPIData(int errorCode, string message )
        {
            this.errorCode = errorCode;
            this.message = message;
        }
    }
}
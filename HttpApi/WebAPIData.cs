
using System;

namespace DFCommonLib.HttpApi
{
    // [Serializable]
    public class WebAPIData : Object
    {
        public int errorCode { get; set; }
        public string message { get; set; }

        public static int CODE_GENERIC = 600;
        public static int CODE_OK = 0;

        public WebAPIData()
        {
            this.errorCode = 0;
            this.message = "";
        }

        public WebAPIData(int errorCode, string message )
        {
            this.errorCode = errorCode;
            this.message = message;
        }

        public static WebAPIData ReturnOK()
        {
            return new WebAPIData(CODE_OK,"");
        }

        public static WebAPIData ReturnFailed(int errorCode, string message)
        {
            return new WebAPIData(errorCode,message);
        }

        public static WebAPIData ReturnGenericFail(string message)
        {
            return new WebAPIData(CODE_GENERIC,message);
        }

        public static WebAPIData ReturnException(Exception exception)
        {
            return new WebAPIData(CODE_GENERIC,exception.ToString());
        }

        public bool IsCodeOK()
        {
            if ( this.errorCode < 300 )
            {
                return true;
            }
            return false;
        }
    }
}
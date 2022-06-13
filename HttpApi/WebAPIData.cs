
using System;

namespace DFCommonLib.HttpApi
{
    // [Serializable]
    public class WebAPIData : Object
    {
        public int errorCode { get; set; }
        public string message { get; set; }

        public WebAPIData()
        {
        }

        public WebAPIData(int errorCode, string message )
        {
            this.errorCode = errorCode;
            this.message = message;
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
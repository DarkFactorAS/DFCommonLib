
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
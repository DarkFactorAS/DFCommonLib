using System;
using Microsoft.Extensions.DependencyInjection;
using DFCommonLib.DataAccess;
using DFCommonLib.Config;
using DFCommonLib.Logger;

namespace DFCommonLib.Utils
{
    public class DFCommonUtil
    {
        public static string CapString( string message, int maxLength)
        {
            if ( message == null || message.Length <= maxLength )
            {
                return message;
            }
            return message.Substring(0,maxLength);
        }
    }
}

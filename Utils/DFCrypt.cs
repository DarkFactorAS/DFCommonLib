
using System.Text;
using System;
using System.Collections.Generic;

namespace DFCommonLib.Utils
{
    public class DFCrypt
    {
        public static string EncryptInput(string plaintext)
        {
            var data = Encoding.UTF8.GetBytes(plaintext);
            return Convert.ToBase64String(data);
        }

        public static string DecryptInput(string encodedString)
        {
            var data = Convert.FromBase64String(encodedString);
            string decodedString = Encoding.UTF8.GetString(data);
            return decodedString;
        }
    }
}

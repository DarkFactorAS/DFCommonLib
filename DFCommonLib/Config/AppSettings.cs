using System;
using System.Collections.Generic;
using System.Text;

namespace DFCommonLib.Config
{
    public class DatabaseConnection
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SslMode { get; set; }
    }       

    public class AppSettings
    {
        public string AppName { get; set; }
        public string SecretKey { get; set; }
        public bool EnableLogging { get; set; }
        public DatabaseConnection DatabaseConnection { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DFCommonLib.Config
{
    public class ConfigurationSettings<T> : ConfigurationSettings where T:Customer,new() 
    {
        public CustomerConfiguration<T> CustomerSettings { get; set; }

        public override CustomerConfiguration GetConfig()
        {
            return CustomerSettings;
        }
    }

    public class ConfigurationSettings
    {
        public AppSettings AppSettings { get; set; }

        public virtual CustomerConfiguration GetConfig()
        {
            return null;
        }
    }
}

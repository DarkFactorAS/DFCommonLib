using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DFCommonLib .Config
{
    public interface IConfigurationHelper
    {
        ConfigurationSettings ConfigurationSettings { get; }
        Customer GetFirstCustomer();
    }

    public class ConfigurationHelper<T> : ConfigurationFactory, IConfigurationHelper
        where T:Customer, new()
    {
        public ConfigurationSettings _configSettings;

        public ConfigurationSettings ConfigurationSettings
        {
            get
            {
                return _configSettings;
            }
        }

        public Customer GetFirstCustomer()
        {
            if (_configSettings != null )
            {
                var customerSetting = _configSettings.GetConfig();
                if ( customerSetting != null )
                {
                    return customerSetting.GetFirstCustomer();
                }
            }
            return null;
        }

        public ConfigurationHelper(IHostEnvironment env) : base(env)
        {
            if (_configSettings == null)
            {
                _configSettings = GetConfigurationFromBuilder(ConfigurationBuilder);
            }
        }

        virtual protected ConfigurationSettings GetConfigurationFromBuilder(IConfiguration builder)
        {
            var configSettings = new ConfigurationSettings<T>();
            builder.Bind(configSettings);
            return configSettings;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace DFCommonLib .Config
{
    public interface IConfigurationHelper
    {
        AppSettings Settings { get; }
    }

    public class ConfigurationHelper<T> : IConfigurationHelper
        where T : AppSettings, new()
    {
        public AppSettings _appSettings;
        IHostEnvironment _env;

        public AppSettings Settings
        {
            get { return _appSettings; }
        }

        public ConfigurationHelper(IHostEnvironment env)
        {
            _env = env;
            if (_appSettings == null)
            {
                var builder = GetConfigurationBuilder();
                _appSettings = GetConfigurationFromBuilder(builder);
            }
        }

        virtual protected AppSettings GetConfigurationFromBuilder(IConfiguration builder)
        {
            var configSettings = new T();
            builder.Bind(configSettings);
            return configSettings;
        }

        private IConfiguration GetConfigurationBuilder()
        {
            string appSettings = "appsettings." + _env.EnvironmentName + ".json";
            string configPath = Path.Combine(Directory.GetCurrentDirectory(), "Config");

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(configPath)
                .AddJsonFile(path: appSettings, optional: false, reloadOnChange: true)
                .Build();

            return config;
        }        
    }
}

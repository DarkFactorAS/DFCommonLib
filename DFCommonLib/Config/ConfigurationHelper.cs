using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using DFCommonLib.Utils;

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

            if (configSettings.IsConfigEncrypted)
            {
                DecryptStringProperties(configSettings, nameof(AppSettings));
            }

            return configSettings;
        }

        private void DecryptStringProperties(object target, string propertyPath)
        {
            if (target == null)
            {
                return;
            }

            var properties = target.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var property in properties)
            {
                var value = property.GetValue(target);
                if (value == null)
                {
                    continue;
                }

                if ( property.Name == "AppName" || property.Name == "AppVersion" )
                {
                    // Allow AppName and AppVersion to be unencrypted
                    continue;
                }

                if (property.PropertyType == typeof(string))
                {
                    var encodedValue = (string)value;
                    if (string.IsNullOrWhiteSpace(encodedValue))
                    {
                        continue;
                    }

                    try
                    {
                        property.SetValue(target, DFCrypt.Decrypt(encodedValue));
                    }
                    catch (FormatException ex)
                    {
                        throw new InvalidOperationException($"Configuration value '{propertyPath}.{property.Name}' is marked as encrypted but is not a valid encoded value.", ex);
                    }

                    continue;
                }

                if (!property.PropertyType.IsValueType)
                {
                    DecryptStringProperties(value, $"{propertyPath}.{property.Name}");
                }
            }
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

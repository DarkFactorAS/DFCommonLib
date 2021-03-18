using System;
using Microsoft.Extensions.DependencyInjection;
using DFCommonLib.DataAccess;
using DFCommonLib.Config;
using DFCommonLib.Logger;

namespace DFCommonLib.Utils
{
    public class DFServices
    {
        IServiceCollection _services;

        public DFServices(IServiceCollection services)
        {
            _services = services;
        }

        public DFServices SetupConfig()
        {
            _services.AddScoped<IConfigurationHelper, ConfigurationHelper>();
            return this;
        }

        public DFServices SetupLogger()
        {
            _services.AddSingleton(typeof(IDFLogger<>), typeof(DFLogger<>));
            return this;
        }

        public DFServices SetupMySql()
        {
            _services.AddScoped<IDbConnectionFactory, LocalMysqlConnectionFactory>();
            _services.BuildServiceProvider();
            _services.AddScoped<IDBPatcher, MySQLDBPatcher>();
            _services.BuildServiceProvider();
            return this;
        }

        public DFServices Setup()
        {
            SetupLogger();
            SetupMySql();
            return this;
        }

        public DFServices LogToConsole(DFLogLevel logLevel)
        {
            DFLogger.AddOutput(logLevel, new ConsoleLogWriter());
            return this;
        }

        public DFServices LogToMySQL(DFLogLevel logLevel)
        {
            var serviceProvider = _services.BuildServiceProvider();
            var connection = (IDbConnectionFactory)serviceProvider.GetService(typeof(IDbConnectionFactory));
            if (connection != null )
            {
                DFLogger.AddOutput(logLevel, new MySqlLogWriter(connection));
            }
            return this;
        }

        public DFServices LogToEvent(DFLogLevel logLevel, string appName)
        {
            DFLogger.AddOutput(logLevel, new EventLogWriter(appName));
            return this;
        }

    }
}

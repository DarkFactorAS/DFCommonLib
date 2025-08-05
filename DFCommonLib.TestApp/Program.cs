using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using DFCommonLib.Config;
using DFCommonLib.Logger;
using DFCommonLib.Utils;
using DFCommonLib.DataAccess;
using TestApp;
using DFCommonLib.TestApp.Programs;
using Google.Protobuf.WellKnownTypes;

namespace DFCommonLibApp
{
    public class Program
    {
        public static string AppName = "DFCommonLib.TestApp";
        public static string AppVersion = "1.0.1";

        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args).Build();

            IDFLogger<Program> logger = new DFLogger<Program>();

            try
            {
                IConfigurationHelper configurationHelper = DFServices.GetService<IConfigurationHelper>();
                var settings = configurationHelper.Settings;
                var msg = string.Format("Connecting to DB : {0}", settings.DatabaseConnection.Server);
                DFLogger.LogOutput(DFLogLevel.INFO, "BotServer", msg);

                new LoggingProgram();
                new DFRestClientProgram();

                IStartupDatabasePatcher startupRepository = DFServices.GetService<IStartupDatabasePatcher>();
                startupRepository.WaitForConnection(1);
                if (startupRepository.RunPatcher())
                {
                    DFLogger.LogOutput(DFLogLevel.INFO, "Startup", "Database patcher ran successfully");
                }
                else
                {
                    DFLogger.LogOutput(DFLogLevel.ERROR, "Startup", "Database patcher failed");
                    Environment.Exit(1);
                    return;
                }

                builder.Run();
            }
            catch (Exception ex)
            {
                logger.LogException("An error occurred during startup", ex);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddTransient<IConfigurationHelper, ConfigurationHelper<TestAppConfig> >();

                new DFServices(services)
                    .SetupLogger()
                    .SetupMySql()
                    .LogToConsole(DFLogLevel.INFO)
                    //.LogToMySQL(DFLogLevel.WARNING)
                    .LogToEvent(DFLogLevel.ERROR, AppName);
                ;
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }
        );
    }
}

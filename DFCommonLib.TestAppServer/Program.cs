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
// using TestApp;
using Google.Protobuf.WellKnownTypes;

using DFCommonLib.TestAppServer.Model;

namespace DFCommonLib.TestAppServer
{
    public class Program
    {
        public static string AppName = "DFCommonLib.TestAppServer";
        public static string AppVersion = "1.0.1";

        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args).Build();

            IDFLogger<Program> logger = new DFLogger<Program>();

            try
            {
                IConfigurationHelper configurationHelper = DFServices.GetService<IConfigurationHelper>();
                var settings = configurationHelper.Settings;
                var msg = string.Format("Connecting to DB : {0}:{1}", settings.DatabaseConnection.Server, settings.DatabaseConnection.Port);
                logger.LogDebug(msg);

                try
                {
                    IStartupDatabasePatcher startupRepository = DFServices.GetService<IStartupDatabasePatcher>();
                    startupRepository.WaitForConnection(1);
                    if (startupRepository.RunPatcher())
                    {
                        logger.LogDebug("Database patcher ran successfully");
                    }
                    else
                    {
                        logger.LogError("Database patcher failed");
                    }
                }
                catch (System.Exception ex)
                {
                    logger.LogException("An error occurred while running the database patcher", ex);
                }

                builder.Run();
            }
            catch (Exception ex)
            {
                logger.LogException("An error occurred while starting the application", ex);
            }
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddTransient<IConfigurationHelper, ConfigurationHelper<AppSettings> >();

                new DFServices(services)
                    .SetupLogger()
                    .SetupMySql()
                    .LogToConsole(DFLogLevel.INFO)
                    .LogToMySQL(DFLogLevel.WARNING)
                    .LogToEvent(DFLogLevel.ERROR, AppName)
                ;
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }
        );

    }
}

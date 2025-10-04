// ...existing code from Program.cs, with namespace and AppName updated...
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
using Google.Protobuf.WellKnownTypes;

namespace DFCommonLib.TestAppClient
{
    public class Program
    {
        public static string AppName = "DFCommonLib.TestAppClient";
        public static string AppVersion = "1.6.5";

        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args).Build();

            try
            {
                IConfigurationHelper configuration = DFServices.GetService<IConfigurationHelper>();
                IDFLogger<Program> logger = DFServices.GetService<IDFLogger<Program>>();

                var program = new TestAppClientProgram(configuration);
                program.Run().ContinueWith(taskReturn =>
                {
                    if (taskReturn.IsCompletedSuccessfully && taskReturn.Result)
                    {
                        logger.LogInfo("TestAppClientProgram run succeeded.");
                        Environment.Exit(0);
                        return;
                    }
                    else if ( taskReturn.Exception != null)
                    {
                        var msg = taskReturn.Exception.ToString();
                        logger.LogError("TestAppClientProgram : Thread exception: {0}", msg);
                        Environment.Exit(-1);
                        return;
                    }
                    logger.LogError($"TestAppClientProgram run failed.");
                    Environment.Exit(-1);
                });

                builder.Run();
            }
            catch (Exception ex)
            {
                DFLogger.LogOutput(DFLogLevel.ERROR, "Startup", ex.ToString());
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddTransient<IConfigurationHelper, ConfigurationHelper<TestAppConfig> >();

                new DFServices(services)
                    .SetupLogger()
//                    .SetupMySql()
                    .LogToConsole(DFLogLevel.INFO)
//                    .LogToMySQL(DFLogLevel.WARNING)
//                    .LogToEvent(DFLogLevel.ERROR, AppName)
                ;
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }
        );
    }
}

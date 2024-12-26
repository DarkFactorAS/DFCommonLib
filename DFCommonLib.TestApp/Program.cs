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

namespace DFCommonLibApp
{
    class Program
    {
        public static string AppName = "DFCommonLibApp";
        public static string AppVersion = "1.0.0";

        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                DFServices.Create(services);

                services.AddTransient<IConfigurationHelper, ConfigurationHelper<Customer> >();

                new DFServices(services)
                    .SetupLogger()
                    .SetupMySql()
                    .LogToConsole(DFLogLevel.INFO)
                    .LogToMySQL(DFLogLevel.WARNING)
                    .LogToEvent(DFLogLevel.ERROR, AppName);
                ;

                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                services.AddTransient<IStartupDatabasePatcher, TestAppDatabasePatcher >();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }
        );
    }
}

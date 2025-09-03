using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DFCommonLib.Utils;
using DFCommonLib.Logger;
using DFCommonLib.DataAccess;
using Swashbuckle.AspNetCore.Swagger;
using DFCommonLib.HttpApi.OAuth2;
using DFCommonLib.TestAppClient.DataAccess;

namespace DFCommonLib.TestAppClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            IDFLogger<Startup> logger = new DFLogger<Startup>();
            logger.Startup(Program.AppName, Program.AppVersion);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            DFServices.Create(services);

            services.AddControllers();
            services.AddMvc();
            services.AddSwaggerGen();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            OAuth2Server.SetupService(services);
            services.AddHttpContextAccessor();
            services.AddTransient(typeof(IStartupDatabasePatcher), typeof(TestAppDatabasePatcher));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}

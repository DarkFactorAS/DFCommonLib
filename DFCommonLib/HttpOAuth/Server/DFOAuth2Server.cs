

using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace DFCommonLib.HttpApi.OAuth2
{
    public class OAuth2Server
    {
        public static void SetupService(IServiceCollection services)
        {
            services.AddTransient<IServerOAuth2Provider, ServerOAuth2Provider>();
            services.AddTransient<IServerOAuth2Repository, ServerOAuth2Repository>();
            services.AddTransient<IServerOAuth2Session, ServerOAuth2Session>();

            services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = OAuth2Static.AuthenticationScheme;
                    options.DefaultChallengeScheme = OAuth2Static.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenHandlers.Add(new DFOAuth2JwtTokenHandler());
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });

        }

        public static void SetupSwaggerApi(string serviceName, IServiceCollection services)
        {
            // register the swagger generator
            services.AddSwaggerGen( c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = serviceName + " API", Version = "v1" });
                c.AddSecurityDefinition(OAuth2Static.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = OAuth2Static.AuthenticationScheme
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = OAuth2Static.AuthenticationScheme
                            },
                            Scheme = OAuth2Static.AuthenticationScheme,
                            Name = OAuth2Static.AuthenticationScheme,
                            In = ParameterLocation.Header
                        },
                        new string[] {}
                    }
                });
            });
        }
    }
}
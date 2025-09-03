

using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DFCommonLib.HttpApi.OAuth2
{
    public class OAuth2Server
    {
        public static void SetupService(IServiceCollection services)
        {
            services.AddTransient<IServerOAuth2Provider, ServerOAuth2Provider>();
            services.AddTransient<IServerOAuth2Repository, ServerOAuth2Repository>();
            services.AddTransient<IServerOAuth2Session, ServerOAuth2Session>();}
    }
}
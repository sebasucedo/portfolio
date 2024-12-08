using Microsoft.Extensions.DependencyInjection;
using portfolio.domain;
using portfolio.infrastructure.httpHandlers;
using portfolio.infrastructure.invertironline;
using portfolio.infrastructure.iol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.infrastructure;

public static class Extensions
{

    public static IServiceCollection AddIolHttpClient(this IServiceCollection services, IolConfig config)
    {

        services.AddHttpClient<IolTokenService>(client =>
        {
            client.BaseAddress = new Uri(config.BaseAddress);
        });
        services.AddHttpClient<IolGateway>(client =>
        {
            client.BaseAddress = new Uri(config.BaseAddress);
        })
        .AddHttpMessageHandler(provider =>
        {
            var handler = new BearerTokenHandler(provider.GetRequiredService<Func<string, ITokenService>>())
            {
                Key = ProvidersConstants.IOL_API
            };
            return handler;
        });
        services.Configure<IolConfig>(iol =>
        {
            iol.BaseAddress = config.BaseAddress;
            iol.Username = config.Username;
            iol.Password = config.Password;
        });

        return services;
    }
}

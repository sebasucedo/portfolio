using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using portfolio.domain;
using portfolio.infrastructure.httpHandlers;
using portfolio.infrastructure.invertironline;
using portfolio.infrastructure.iol;
using portfolio.infrastructure.ppi;
using StackExchange.Redis;
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
        services.AddHttpClient<IIolGateway, IolGateway>(client =>
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

    public static IServiceCollection AddPPiHttpClient(this  IServiceCollection services, PpiConfig config)
    {
        services.AddHttpClient<PpiTokenService>(client =>
        {
            client.BaseAddress = new Uri(config.BaseAddress);
            client.DefaultRequestHeaders.Add("AuthorizedClient", config.AuthorizedClient);
            client.DefaultRequestHeaders.Add("ClientKey", config.ClientKey);
            client.DefaultRequestHeaders.Add("ApiKey", config.ApiKey);
            client.DefaultRequestHeaders.Add("ApiSecret", config.ApiSecret);
        });
        services.AddHttpClient<IPpiGateway, PpiGateway>(client =>
        {
            client.BaseAddress = new Uri(config.BaseAddress);
            client.DefaultRequestHeaders.Add("AuthorizedClient", config.AuthorizedClient);
            client.DefaultRequestHeaders.Add("ClientKey", config.ClientKey);
        })
        .AddHttpMessageHandler(provider =>
        {
            var handler = new BearerTokenHandler(provider.GetRequiredService<Func<string, ITokenService>>())
            {
                Key = ProvidersConstants.PPI_API
            };
            return handler;
        });
        services.AddSingleton(Options.Create(config));

        return services;
    }

    public static IServiceCollection AddCache(this IServiceCollection services, RedisConfig redisConfig)
    {
        services.AddSingleton(Options.Create(redisConfig));

        var configurationOptions = new ConfigurationOptions
        {
            EndPoints = { { redisConfig.Host, redisConfig.Port } },
            User = redisConfig.User,
            Password = redisConfig.Password,
        };
        IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);

        services.AddStackExchangeRedisCache(options =>
        {
            options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer);
            options.InstanceName = redisConfig.InstanceName;
        });

        return services;
    }
}

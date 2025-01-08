using portfolio.domain;
using portfolio.infrastructure;
using portfolio.infrastructure.alpaca;
using portfolio.infrastructure.httpHandlers;

namespace portfolio.api;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var config = configuration.Get();

        services.AddHttpClient<IAlpacaGateway, AlpacaGateway>(provider =>
        {
            provider.BaseAddress = new Uri(config.Alpaca.BaseAddress);
            provider.DefaultRequestHeaders.Add(infrastructure.Constants.Keys.ALPACA_KEY, config.Alpaca.Key);
            provider.DefaultRequestHeaders.Add(infrastructure.Constants.Keys.ALPACA_SECRET, config.Alpaca.Secret);
        });

        services.AddBearerTokenHandler();
        services.AddIolHttpClient(config.Iol);
        services.AddPPiHttpClient(config.Ppi);
        services.AddCache(config.Redis);

        services.AddTransient<ICacheInterceptor, CacheInterceptor>();
        services.AddTransient<Service>();

        return services;
    }
}

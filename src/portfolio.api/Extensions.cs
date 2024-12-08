using portfolio.infrastructure;
using portfolio.infrastructure.alpaca;
using portfolio.infrastructure.httpHandlers;

namespace portfolio.api;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var config = configuration.Get();

        services.AddHttpClient<AlpacaGateway>(provider =>
        {
            provider.BaseAddress = new Uri(config.Alpaca.BaseAddress);
            provider.DefaultRequestHeaders.Add(Constants.Keys.ALPACA_KEY, config.Alpaca.Key);
            provider.DefaultRequestHeaders.Add(Constants.Keys.ALPACA_SECRET, config.Alpaca.Secret);
        });

        services.AddBearerTokenHandler();
        services.AddIolHttpClient(config.Iol);

        return services;
    }
}

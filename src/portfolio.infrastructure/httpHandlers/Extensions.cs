using Microsoft.Extensions.DependencyInjection;
using portfolio.domain;
using portfolio.infrastructure.invertironline;
using portfolio.infrastructure.iol;
using portfolio.infrastructure.ppi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.infrastructure.httpHandlers;

public static class Extensions
{
    public static IServiceCollection AddBearerTokenHandler(this IServiceCollection services)
    {
        services.AddTransient<Func<string, ITokenService>>(serviceProvider => key =>
        {
            return key switch
            {
                ProvidersConstants.IOL_API => serviceProvider.GetRequiredService<IolTokenService>(),
                ProvidersConstants.PPI_API => serviceProvider.GetRequiredService<PpiTokenService>(),
                _ => throw new KeyNotFoundException("The requested token service does not exist for the provided key."),
            };
        });

        services.AddTransient<BearerTokenHandler>();

        return services;
    }
}

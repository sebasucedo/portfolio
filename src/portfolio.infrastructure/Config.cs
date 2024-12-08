using Microsoft.Extensions.Configuration;
using portfolio.infrastructure.alpaca;
using portfolio.infrastructure.iol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.infrastructure;

public class Config(IConfiguration config)
{
    public AlpacaConfig Alpaca { get; set; } = config.GetSection(nameof(Alpaca)).Get<AlpacaConfig>()!;
    public IolConfig Iol { get; set; } = config.GetSection(nameof(Iol)).Get<IolConfig>()!;
}

public static class ConfigExtensions
{
    public static Config Get(this IConfiguration configuration) => new(configuration);
}



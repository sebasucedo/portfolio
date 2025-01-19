using Microsoft.Extensions.Configuration;
using portfolio.infrastructure.alpaca;
using portfolio.infrastructure.iol;
using portfolio.infrastructure.ppi;
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
    public PpiConfig Ppi { get; set; } = config.GetSection(nameof(Ppi)).Get<PpiConfig>()!;
    public RedisConfig Redis { get; set; } = config.GetSection(nameof(Redis)).Get<RedisConfig>()!;
}

public class RedisConfig
{
    public required string InstanceName { get; set; }
    public required string Host { get; set; }
    public int Port { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public int DefaultTtl { get; set; }
}

public static class ConfigExtensions
{
    public static Config Get(this IConfiguration configuration) => new(configuration);
}



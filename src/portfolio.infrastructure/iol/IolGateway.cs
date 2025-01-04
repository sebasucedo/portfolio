using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using portfolio.domain;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace portfolio.infrastructure.iol;

public class IolGateway(HttpClient httpClient, 
                        IDistributedCache cache,
                        IOptions<RedisConfig> redisConfig)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IDistributedCache _cache = cache;
    private readonly DistributedCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(redisConfig.Value.DefaultTtl)
    };

    readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

    public async Task<Ledger> GetLedger()
    {
        var endpoint = "/api/estadocuenta";
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<LedgerDTO>(content, options);

            var ledger = new Ledger { Name = "Iol" };
            if (dto is not null)
            {
                foreach (var cuenta in dto.Cuentas)
                {
                    var currency = new Currency
                    {
                        Cash = Convert.ToDecimal(cuenta.Saldo),
                        BuyingPower = Convert.ToDecimal(cuenta.Disponible),
                    };

                    var key = cuenta.Tipo switch
                    {
                        "inversion_Estados_Unidos_Dolares" => domain.Constants.Currencies.USD,
                        "inversion_Argentina_Dolares" => domain.Constants.Currencies.MEP,
                        "inversion_Argentina_Pesos" => domain.Constants.Currencies.ARS,
                        _ => throw new ArgumentException("tipo not found")
                    };

                    ledger.Currencies.Add(key, currency);
                }
            }

            return ledger;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error getting ledger from Iol");
            throw;
        }
    }

    const string PREFIX_CACHE_KEY = "/iol";
    async Task<IEnumerable<Position>?> GetPositionsFromCache(string cacheKey)
    {
        cacheKey = PREFIX_CACHE_KEY + cacheKey;
        try
        {
            var jsonData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(jsonData))
                return JsonSerializer.Deserialize<IEnumerable<Position>>(jsonData)!;
        }
        catch (RedisConnectionException ex)
        {
            Serilog.Log.Error(ex, "Error connection to Redis trying to get positions from cache");
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error getting positions from cache");
        }
        return null;
    }

    async Task CachePositions(IEnumerable<Position> positions, string cacheKey)
    {
        cacheKey = PREFIX_CACHE_KEY + cacheKey;
        try
        {
            var jsonData = JsonSerializer.Serialize(positions);

            await _cache.SetStringAsync(cacheKey, jsonData, _cacheOptions);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error caching positions");
        }
    }

    public async Task<IEnumerable<Position>> GetPositions()
    {
        var endpoint = "/api/portafolio";
        try
        {
            var fromCache = await GetPositionsFromCache(endpoint);
            if (fromCache is not null)
                return fromCache;

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var portfolio = JsonSerializer.Deserialize<PortfolioDTO>(content, options);

            if (portfolio is null || portfolio.Activos is null)
                return [];

            var positions = portfolio.Activos.Select(x => new Position
            {
                DataSource = domain.Constants.DataSoruces.IOL,
                Symbol = x.Titulo.Simbolo,
                Exchange = x.Titulo.Mercado,
                AssetClass = x.Titulo.Tipo,
                Quantity = x.Cantidad,
                Currency = x.Titulo.Moneda switch
                {
                    "dolar_Estadounidense" => domain.Constants.Currencies.USD,
                    "peso_Argentino" => domain.Constants.Currencies.ARS,
                    _ => string.Empty,
                },
                Price = x.UltimoPrecio,
                MarketValue = x.Valorizado,
            }).ToList();

            await CachePositions(positions, endpoint);

            return positions;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error getting portfolio from Iol");
            throw;
        }
    }
}

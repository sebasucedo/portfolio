using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using portfolio.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace portfolio.infrastructure;

public class CacheInterceptor(IDistributedCache cache,
                              IOptions<RedisConfig> redisConfig) : ICacheInterceptor
{
    private readonly IDistributedCache _cache = cache;
    private readonly int _durationMinutesDefault = redisConfig.Value.DefaultTtl;

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> method, string key) => await ExecuteAsync(method, key, _durationMinutesDefault);

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> method, string key, int durationMinutes)
    {
        var cachedValue = await _cache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(cachedValue))
            return JsonSerializer.Deserialize<T>(cachedValue)!;

        var result = await method();

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(durationMinutes)
        };

        await _cache.SetStringAsync(key, JsonSerializer.Serialize(result), options);

        return result;
    }
}
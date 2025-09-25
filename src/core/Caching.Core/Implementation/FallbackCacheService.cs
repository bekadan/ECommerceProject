using Caching.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace Caching.Core.Implementation;

public class FallbackCacheService : ICacheService
{
    private readonly ICacheService _redisCache;
    private readonly ICacheService _memoryCache;
    private readonly ILogger<FallbackCacheService> _logger;

    public FallbackCacheService(
        RedisCacheService redisCache,
        InMemoryCacheService memoryCache,
        ILogger<FallbackCacheService> logger)
    {
        _redisCache = redisCache;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _redisCache.GetAsync<T>(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis unavailable, falling back to MemoryCache for GET {Key}", key);
            return await _memoryCache.GetAsync<T>(key, cancellationToken);
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await _redisCache.SetAsync(key, value, expiration, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis unavailable, falling back to MemoryCache for SET {Key}", key);
            await _memoryCache.SetAsync(key, value, expiration, cancellationToken);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _redisCache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis unavailable, falling back to MemoryCache for REMOVE {Key}", key);
            await _memoryCache.RemoveAsync(key, cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _redisCache.ExistsAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis unavailable, falling back to MemoryCache for EXISTS {Key}", key);
            return await _memoryCache.ExistsAsync(key, cancellationToken);
        }
    }

    public async Task<T> GetOrAddAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? expiration = null,
            CancellationToken cancellationToken = default)
    {
        try
        {
            // Try Redis first
            return await _redisCache.GetOrAddAsync(key, factory, expiration, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis unavailable, falling back to MemoryCache for GETORADD {Key}", key);
            return await _memoryCache.GetOrAddAsync(key, factory, expiration, cancellationToken);
        }
    }
}

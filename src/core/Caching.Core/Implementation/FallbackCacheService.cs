using Caching.Core.Abstractions;
using System.Collections.Concurrent;

namespace Caching.Core.Implementation;

public class FallbackCacheService : ICacheService
{

    private readonly ConcurrentDictionary<string, (object Value, DateTime? Expiration)> _cache
            = new ConcurrentDictionary<string, (object, DateTime?)>();

    public FallbackCacheService()
    {
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.Expiration.HasValue && entry.Expiration.Value < DateTime.UtcNow)
            {
                _cache.TryRemove(key, out _);
                return Task.FromResult(default(T));
            }
            return Task.FromResult((T?)entry.Value);
        }
        return Task.FromResult(default(T));
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var expirationTime = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : (DateTime?)null;
        _cache[key] = (value!, expirationTime);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.Expiration.HasValue && entry.Expiration.Value < DateTime.UtcNow)
            {
                _cache.TryRemove(key, out _);
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(key, cancellationToken);
        if (exists)
        {
            var cached = await GetAsync<T>(key, cancellationToken);
            if (cached != null) return cached;
        }

        var value = await factory();
        await SetAsync(key, value, expiration, cancellationToken);
        return value;
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var keysToRemove = _cache.Keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
        foreach (var key in keysToRemove)
            _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public string AddPrefix(string prefix, string key)
    {
        if (string.IsNullOrWhiteSpace(prefix)) return key;
        return prefix.EndsWith(":") ? $"{prefix}{key}" : $"{prefix}:{key}";
    }
}

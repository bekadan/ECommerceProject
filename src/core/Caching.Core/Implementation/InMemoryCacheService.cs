using Caching.Core.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Caching.Core.Implementation;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly HashSet<string> _keys = new(); // track keys for prefix-based removal
    private readonly object _lock = new();

    public InMemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue)
            options.AbsoluteExpirationRelativeToNow = expiration;

        lock (_lock) _keys.Add(key);

        _cache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);
        lock (_lock) _keys.Remove(key);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_cache.TryGetValue(key, out _));
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

    public string AddPrefix(string prefix, string key)
    {
        if (string.IsNullOrWhiteSpace(prefix)) return key;
        return prefix.EndsWith(":") ? $"{prefix}{key}" : $"{prefix}:{key}";
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        List<string> keysToRemove;
        lock (_lock)
        {
            keysToRemove = _keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
            lock (_lock) _keys.Remove(key);
        }

        return Task.CompletedTask;
    }
}

using Caching.Core.Abstractions;
using StackExchange.Redis;
using System.Text.Json;

namespace Caching.Core.Implementation;

public class RedisCacheService : ICacheService
{
    
    private readonly IDatabase _database;
  
    private readonly IConnectionMultiplexer _connection;

    public RedisCacheService(IConnectionMultiplexer connection, IDatabase database)
    {
        _connection = connection;
        _database = database;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, json, expiration);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _database.KeyExistsAsync(key);
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

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var endpoints = _connection.GetEndPoints();
        foreach (var endpoint in endpoints)
        {
            var server = _connection.GetServer(endpoint);
            var keys = server.Keys(pattern: $"{prefix}*");
            var deleteTasks = keys.Select(k => _database.KeyDeleteAsync(k));
            await Task.WhenAll(deleteTasks);
        }
    }
}

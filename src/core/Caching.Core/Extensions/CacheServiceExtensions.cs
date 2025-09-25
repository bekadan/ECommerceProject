using Caching.Core.Abstractions;

namespace Caching.Core.Extensions;

public static class CacheServiceExtensions
{
    /// <summary>
    /// Gets a paginated value from cache, or adds it if missing.
    /// </summary>
    public static Task<T> GetOrAddPaginatedAsync<T>(
        this ICacheService cacheService,
        string prefix,
        int page,
        int size,
        Func<Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        string key = $"{prefix}:page:{page}-size:{size}";
        return cacheService.GetOrAddAsync(key, factory, expiration, cancellationToken);
    }

    /// <summary>
    /// Convenience method to remove all paginated cache for a given prefix.
    /// </summary>
    public static Task RemovePaginatedByPrefixAsync(
        this ICacheService cacheService,
        string prefix,
        CancellationToken cancellationToken = default)
    {
        return cacheService.RemoveByPrefixAsync($"{prefix}:page:", cancellationToken);
    }
}

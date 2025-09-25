using Caching.Core.Abstractions;
using Caching.Core.Implementation;
using Caching.Core.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Caching.Core;
public static class DependencyInjection
{
    public static IServiceCollection AddCachingCore(
            this IServiceCollection services,
            IConfiguration configuration,
            bool useRedis = false,
            bool enableFallback = true)
    {
        services.Configure<CacheOptions>(configuration.GetSection("CacheOptions"));
        services.AddSingleton<ICacheKeyProvider, DefaultCacheKeyProvider>();

        // Register memory cache always
        services.AddMemoryCache();
        services.AddSingleton<InMemoryCacheService>();

        if (useRedis)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
            });
            services.AddSingleton<RedisCacheService>();

            if (enableFallback)
            {
                // ✅ Use Fallback (preferred)
                services.AddSingleton<ICacheService, FallbackCacheService>();
            }
            else
            {
                // ✅ Use Redis only
                services.AddSingleton<ICacheService, RedisCacheService>();
            }
        }
        else
        {
            // ✅ Use Memory only
            services.AddSingleton<ICacheService, InMemoryCacheService>();
        }

        return services;
    }
}


/*
 "CacheOptions": {
  "DefaultExpiration": "00:10:00",
  "KeyPrefix": "ecommerce",
  "ServiceName": "catalog"
}

{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  },
  "CacheOptions": {
    "DefaultExpirationSeconds": 300
  }
}

ecommerce:catalog:product:7b2a90a5-3bc3-42fa-a0d1-d6fb86a1a9aa
 
 */

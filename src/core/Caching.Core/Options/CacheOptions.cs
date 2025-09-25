namespace Caching.Core.Options;

public class CacheOptions
{
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(5);
    public string KeyPrefix { get; set; } = "app:";
    public string ServiceName { get; set; } = ""; // e.g. "catalog", "basket"
}

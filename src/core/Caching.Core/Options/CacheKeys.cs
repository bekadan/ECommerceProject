namespace Caching.Core.Options;

public static class CacheKeys
{
    public const string Products = "products";
    public const string Categories = "categories";

    public static string ProductById(Guid id) => $"{Products}:{id}";
    public static string ProductPaged(int page, int size) => $"{Products}:page:{page}-size:{size}";
    public static string CategoryById(Guid id) => $"{Categories}:{id}";
    public static string CategoryPaged(int page, int size) => $"{Categories}:page:{page}-size:{size}";
}

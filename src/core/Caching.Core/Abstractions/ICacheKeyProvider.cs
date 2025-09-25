namespace Caching.Core.Abstractions;

public interface ICacheKeyProvider
{
    string Build(string entity, params object[] parts);
}

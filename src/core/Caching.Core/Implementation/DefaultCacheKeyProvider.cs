using Caching.Core.Abstractions;
using Caching.Core.Options;
using Microsoft.Extensions.Options;
using System.Text;

namespace Caching.Core.Implementation;

public class DefaultCacheKeyProvider : ICacheKeyProvider
{
    private readonly CacheOptions _options;

    public DefaultCacheKeyProvider(IOptions<CacheOptions> options)
    {
        _options = options.Value;
    }

    public string Build(string entity, params object[] parts)
    {
        var sb = new StringBuilder();

        // Add global prefix (e.g. "app:")
        if (!string.IsNullOrWhiteSpace(_options.KeyPrefix))
            sb.Append(_options.KeyPrefix.TrimEnd(':')).Append(':');

        // Add service name (from options)
        if (!string.IsNullOrWhiteSpace(_options.ServiceName))
            sb.Append(_options.ServiceName.TrimEnd(':')).Append(':');

        // Add entity (e.g., "product")
        sb.Append(entity.ToLowerInvariant());

        // Add extra parts (e.g., ID, filters, etc.)
        foreach (var part in parts)
        {
            if (part != null)
            {
                sb.Append(':').Append(part.ToString()?.ToLowerInvariant());
            }
        }

        return sb.ToString();
    }
}

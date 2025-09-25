using Core.Utilities.Abstractions;

namespace Core.Utilities.Providers;

public class GuidProvider : IGuidProvider
{
    public Guid NewGuid() => Guid.NewGuid();
}

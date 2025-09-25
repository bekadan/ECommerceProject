using Microsoft.Extensions.Configuration;

namespace Core.Utilities.Configuration;

public static class ConfigurationHelper
{
    public static T GetSection<T>(this IConfiguration configuration, string sectionName)
        where T : new()
    {
        var section = new T();
        configuration.GetSection(sectionName).Bind(section);
        return section;
    }
}

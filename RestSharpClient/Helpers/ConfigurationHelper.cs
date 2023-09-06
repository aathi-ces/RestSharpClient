using Microsoft.Extensions.Configuration;
using Settings;

namespace Helpers;
public static class ConfigurationHelper
{
    public static AppSettings BuildConfiguration()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json").Build().Get<AppSettings>();
        return config!;
    }
}

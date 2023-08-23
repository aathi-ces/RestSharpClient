using System.IO;
// using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Settings;

namespace Helpers
{
    public class ConfigurationHelper
    {
        // public static IConfigurationRoot ConfigurationRoot { get; private set; }

        // public static string BaseUrl => ConfigurationRoot[nameof(BaseUrl)];

        public static AppSettings BuildConfiguration()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build().Get<AppSettings>();
            return config!;
        }
    }
}

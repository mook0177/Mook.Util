#if NETSTANDARD2_0
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Mook.Util
{
    public class AppSettingsHelper
    {
        public static IConfiguration Configuration { get; set; }

        static AppSettingsHelper()
        {
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();
        }
    }
}
#endif
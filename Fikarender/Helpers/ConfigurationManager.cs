using Microsoft.Extensions.Configuration;

namespace Helpers
{
    public class ConfigurationManager
    {
        public static IConfiguration AppSetting { get; }

        static ConfigurationManager()
        {
            AppSetting = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
        }
    }
}

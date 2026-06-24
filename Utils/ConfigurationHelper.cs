using Microsoft.Extensions.Configuration;
using System.IO;

namespace DronSimulator.Utils
{
    public static class ConfigurationHelper
    {
        private static IConfigurationRoot _configuration;

        public static void LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }

        public static string GetPostgresConnectionString()
        {
            return _configuration.GetConnectionString("PostgreSQL");
        }
    }
}
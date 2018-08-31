using Microsoft.Extensions.Configuration;

namespace OrdersApi.Tests
{
    public class ConfigHelper
    {
        public static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings.Development.json", false, true)
                .Build();
        }
    }
}
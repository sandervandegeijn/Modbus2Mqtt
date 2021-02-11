using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Modbus2Mqtt.Infrastructure.YmlConfiguration
{
    public class ConfigurationFactory
    {
        private readonly ILogger<ConfigurationFactory> _logger;

        public ConfigurationFactory(ILogger<ConfigurationFactory> logger)
        {
            _logger = logger;
        }
        
        private static YmlConfiguration.Configuration.Configuration Configuration { get; set; }

        public static YmlConfiguration.Configuration.Configuration GetConfiguration()
        {
            //Singleton
            if (Configuration != null)
            {
                return Configuration;
            }

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "configuration.yml");

            var yml = File.ReadAllText(path);
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance) 
                .Build();
            
            var config = deserializer.Deserialize<YmlConfiguration.Configuration.Configuration>(yml);
            
            foreach (var slave in config.Slave)
            {
                slave.DeviceDefition = DeviceFactory.GetDevice(slave.Type);
            }

            Configuration = config;
            return config;
        }
    }
}
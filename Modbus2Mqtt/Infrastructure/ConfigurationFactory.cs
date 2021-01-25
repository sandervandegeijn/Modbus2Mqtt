using System.IO;
using System.Reflection;
using NLog;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Modbus2Mqtt.Infrastructure
{
    public static class ConfigurationFactory
    {
        private static Configuration.Configuration Configuration { get; set; }
        
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public static Configuration.Configuration GetConfiguration()
        {
            //Singleton
            if (Configuration != null)
            {
                return Configuration;
            }

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "configuration.yml");
            Logger.Info("Trying to parse configuration: "+path);
            
            var yml = File.ReadAllText(path);
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance) 
                .Build();
            
            var config = deserializer.Deserialize<Configuration.Configuration>(yml);
            
            foreach (var slave in config.Slaves)
            {
                slave.Device = DeviceFactory.GetDevice(slave.Type);
            }

            Configuration = config;
            return config;
        }
    }
}
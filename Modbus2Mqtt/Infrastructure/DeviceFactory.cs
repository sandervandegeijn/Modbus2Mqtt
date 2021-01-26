using System.IO;
using System.Reflection;
using NLog;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Modbus2Mqtt.Infrastructure
{
    public static class DeviceFactory
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public static DeviceDefinition.DeviceDefition GetDevice(string name)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DeviceTemplates/" + name + ".yml");
            Logger.Info("Trying to parse device template: "+path);
            var yml = File.ReadAllText(path);
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            return deserializer.Deserialize<DeviceDefinition.DeviceDefition>(yml);
        }
    }
}
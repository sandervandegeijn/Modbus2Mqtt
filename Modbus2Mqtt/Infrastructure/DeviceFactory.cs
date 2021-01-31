using System.IO;
using System.Reflection;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.DeviceDefinition;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Modbus2Mqtt.Infrastructure
{
    public static class DeviceFactory
    {

        public static DeviceDefition GetDevice(string name)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DeviceTemplates/" + name + ".yml");
            //Logger.Info("Trying to parse device template: "+path);
            var yml = File.ReadAllText(path);
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            return deserializer.Deserialize<DeviceDefition>(yml);
        }
    }
}
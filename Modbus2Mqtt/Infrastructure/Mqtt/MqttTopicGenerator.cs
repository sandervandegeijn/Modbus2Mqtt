using System.Text.RegularExpressions;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.Configuration;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.DeviceDefinition;

namespace Modbus2Mqtt.Infrastructure.Mqtt
{
    public class MqttTopicGenerator
    {
        private readonly Configuration _configuration;

        public MqttTopicGenerator(Configuration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateStateTopic(Slave slave, Register register)
        {
            return $"{_configuration.Mqtt.MainTopic}/get/{StripNonAlphaNumeric(slave.Name)}/{StripNonAlphaNumeric(register.Name)}";
        }

        public string GenerateAvailabilityTopic()
        {
            return $"{_configuration.Mqtt.MainTopic}/status";
        }
        
        
        public string GenerateHomeAssistantAutodiscoveryTopic(Slave slave, Register register)
        {
            return
                $"{_configuration.HomeassistantAutoDiscoveryPrefix}/sensor/{StripNonAlphaNumeric(slave.Name)}/{StripNonAlphaNumeric(slave.Name)}-{StripNonAlphaNumeric(register.Name)}/config";
        }
        
        public static string StripNonAlphaNumeric(string input)
        {
            return Regex.Replace(input, @"[^a-zA-Z0-9]+", "-");
        }
    }
}
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
            return $"{_configuration.Mqtt.MainTopic}/get/{slave.GetStrippedName()}/{register.GetStrippedName()}";
        }

        public string GenerateAvailabilityTopic()
        {
            return $"{_configuration.Mqtt.MainTopic}/status";
        }
        
        
        public string GenerateHomeAssistantAutodiscoveryTopic(Slave slave, Register register)
        {
            return
                $"{_configuration.HomeassistantAutoDiscoveryPrefix}/sensor/{slave.GetStrippedName()}/{slave.GetStrippedName()}-{register.GetStrippedName()}/config";
        }
        
    }
}
using System.Collections.Generic;
using System.Linq;
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

        public List<string> GenerateTopicsForIncomingMqttTrafficForAllSlaves()
        {
            var topics = new List<string>();
            
            foreach (var slave in _configuration.Slave)
            {
                var registers = (from r in slave.DeviceDefition.Registers
                    where r.Function == EnumModbusFunction.write_multiple_coils ||
                    r.Function == EnumModbusFunction.write_multiple_holding_registers ||
                    r.Function == EnumModbusFunction.write_single_coil ||
                    r.Function == EnumModbusFunction.write_single_holding_register
                    select r).ToList();

                foreach (var register in registers)
                {
                    topics.Add($"{_configuration.Mqtt.MainTopic}/set/{slave.Name}/{register.Name}");
                }
            }

            return topics;
        }
        
        public string GenerateStateTopic(Slave slave, Register register)
        {
            return $"{_configuration.Mqtt.MainTopic.ToLower()}/get/{slave.GetStrippedName().ToLower()}/{register.GetStrippedName().ToLower()}";
        }

        public string GenerateAvailabilityTopic()
        {
            return $"{_configuration.Mqtt.MainTopic.ToLower()}/status";
        }
        
        public string GenerateHomeAssistantAutodiscoveryTopic(Slave slave, Register register)
        {
            return
                $"{_configuration.HomeassistantAutoDiscoveryPrefix}/sensor/{slave.GetStrippedName().ToLower()}/{slave.GetStrippedName().ToLower()}-{register.GetStrippedName().ToLower()}/config";
        }
        
    }
}
using Modbus2Mqtt.Infrastructure.Configuration;
using Modbus2Mqtt.Infrastructure.DeviceDefinition;

namespace Modbus2Mqtt.Eventing.ModbusRequest
{
    public class ModbusRequest
    {
        public Register Register { get; set; }
        
        public Slave Slave { get; set; }
    }
}
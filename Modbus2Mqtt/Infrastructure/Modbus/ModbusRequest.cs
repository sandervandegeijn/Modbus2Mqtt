using System;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.Configuration;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.DeviceDefinition;

namespace Modbus2Mqtt.Infrastructure.Modbus
{
    public class ModbusRequest
    {
        public Register Register { get; set; }
        
        public Slave Slave { get; set; }
        
        public DateTime NextExecutionTime { get; set; }
    }
}
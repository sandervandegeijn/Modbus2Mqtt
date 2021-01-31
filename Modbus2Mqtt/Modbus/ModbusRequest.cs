using System;
using Modbus2Mqtt.Infrastructure.Configuration;
using Modbus2Mqtt.Infrastructure.DeviceDefinition;

namespace Modbus2Mqtt.Modbus
{
    public class ModbusRequest
    {
        public Register Register { get; set; }
        
        public Slave Slave { get; set; }
        
        public DateTime NextExecutionTime { get; set; }
    }
}
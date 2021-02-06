using MediatR;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.Configuration;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.DeviceDefinition;

namespace Modbus2Mqtt.Eventing.ModbusResult
{
    public class ModbusCoilResultEvent : INotification
    {
        public Register Register { get; set; }
        
        public Slave Slave { get; set; }
        
        public bool[] Result { get; set; }
    }
}
using MediatR;
using Modbus2Mqtt.Infrastructure.Configuration;
using Modbus2Mqtt.Infrastructure.DeviceDefinition;

namespace Modbus2Mqtt.Eventing.ModbusResult
{
    public class ModbusCoilResult : INotification
    {
        public Register Register { get; set; }
        
        public Slave Slave { get; set; }
        
        public bool[] Result { get; set; }
    }
}
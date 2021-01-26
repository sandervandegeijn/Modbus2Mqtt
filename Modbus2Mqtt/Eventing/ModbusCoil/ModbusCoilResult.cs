using MediatR;
using Modbus2Mqtt.Infrastructure.DeviceDefinition;

namespace Modbus2Mqtt.Eventing.ModbusCoil
{
    public class ModbusCoilResult : INotification
    {
        public Register Register { get; set; }
        
        public int[] Result { get; set; }
    }
}
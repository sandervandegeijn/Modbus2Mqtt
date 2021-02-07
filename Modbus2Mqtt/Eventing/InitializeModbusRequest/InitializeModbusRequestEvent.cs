using MediatR;
using Modbus2Mqtt.Infrastructure.Modbus;

namespace Modbus2Mqtt.Eventing.InitializeModbusRequest
{
    public class InitializeModbusRequestEvent : INotification
    {
        public ModbusRequest ModbusRequest { get; set; }

        public InitializeModbusRequestEvent(ModbusRequest modbusRequest)
        {
            ModbusRequest = modbusRequest;
        }
    }
}
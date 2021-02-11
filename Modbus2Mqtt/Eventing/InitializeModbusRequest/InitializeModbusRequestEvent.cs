using MediatR;
using Modbus2Mqtt.Infrastructure.Modbus;

namespace Modbus2Mqtt.Eventing.InitializeModbusRequest
{
    public class InitializeModbusRequestEvent : INotification
    {
        public ModbusRequest ModbusReadRequest { get; set; }

        public InitializeModbusRequestEvent(ModbusRequest modbusReadRequest)
        {
            ModbusReadRequest = modbusReadRequest;
        }
    }
}
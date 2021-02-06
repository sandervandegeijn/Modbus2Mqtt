using MediatR;
using Modbus2Mqtt.Infrastructure.Modbus;

namespace Modbus2Mqtt.Eventing.NewModbusRequest
{
    public class NewModbusRequestEvent : INotification
    {
        public ModbusRequest ModbusRequest { get; set; }

        public NewModbusRequestEvent(ModbusRequest modbusRequest)
        {
            ModbusRequest = modbusRequest;
        }
    }
}
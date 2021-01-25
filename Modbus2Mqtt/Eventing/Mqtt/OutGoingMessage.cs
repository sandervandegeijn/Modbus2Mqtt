using MediatR;
using Modbus2Mqtt.Infrastructure.Configuration;
using Modbus2Mqtt.Infrastructure.Device;

namespace Modbus2Mqtt.Eventing.Mqtt
{
    public class OutGoingMessage : INotification
    {
        public Slaves Slave { get; set; }
        
        public Register Register { get; set; }
        
        public string Message { get; set; }
    }
}
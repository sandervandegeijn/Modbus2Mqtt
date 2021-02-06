using MediatR;

namespace Modbus2Mqtt.Eventing.Mqtt
{
    public class IncomingMessageEvent : INotification
    {
        public string Topic { get; set; }
        
        public string Message { get; set; }
    }
}
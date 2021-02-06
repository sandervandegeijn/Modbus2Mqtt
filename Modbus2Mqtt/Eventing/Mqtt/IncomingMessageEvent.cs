namespace Modbus2Mqtt.Eventing.Mqtt
{
    public class IncomingMessageEvent
    {
        public string Topic { get; set; }
        
        public string Message { get; set; }
    }
}
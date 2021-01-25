namespace Modbus2Mqtt.Eventing.Mqtt
{
    public class IncomingMessage
    {
        public string Topic { get; set; }
        
        public string Message { get; set; }
    }
}
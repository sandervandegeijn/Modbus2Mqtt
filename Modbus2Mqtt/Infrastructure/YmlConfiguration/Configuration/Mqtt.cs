namespace Modbus2Mqtt.Infrastructure.YmlConfiguration.Configuration
{
    public class Mqtt
    {
        public string Server { get; set; }
        
        public int Port { get; set; }
        
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public bool Tls { get; set; }
        
        public string ClientId { get; set; }
        
        public string MainTopic { get; set; }
    }
}
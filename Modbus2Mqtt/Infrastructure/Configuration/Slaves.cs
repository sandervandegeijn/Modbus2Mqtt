namespace Modbus2Mqtt.Infrastructure.Configuration
{
    public class Slaves
    {
        public string Name { get; set; }
        
        public string Type { get; set; }
        
        public int SlaveId { get; set; }
        
        public int PollingInterval { get; set; }
        
        public Device.Device Device { get; set; }
    }
}
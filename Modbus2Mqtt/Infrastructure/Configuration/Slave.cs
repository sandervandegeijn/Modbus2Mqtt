namespace Modbus2Mqtt.Infrastructure.Configuration
{
    public class Slave
    {
        public string Name { get; set; }
        
        public string Type { get; set; }
        
        public int SlaveId { get; set; }
        
        public int PollingInterval { get; set; }
        
        public DeviceDefinition.DeviceDefition DeviceDefition { get; set; }
    }
}
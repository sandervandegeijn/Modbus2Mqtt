namespace Modbus2Mqtt.Infrastructure.YmlConfiguration.Configuration
{
    public class Slave
    {
        public string Name { get; set; }
        
        public string Type { get; set; }
        
        public int SlaveId { get; set; }
        
        public int PollingInterval { get; set; }
        
        public int Priority { get; set; }
        
        public string Exclude { get; set; }
        
        public string Include { get; set; }
        
        public DeviceDefinition.DeviceDefition DeviceDefition { get; set; }

        public string GetStrippedName()
        {
            return Name.StripNonAlphaNumeric().ToLower();
        }
    }
}
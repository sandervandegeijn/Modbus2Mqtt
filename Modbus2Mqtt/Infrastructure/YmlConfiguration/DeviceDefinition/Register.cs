namespace Modbus2Mqtt.Infrastructure.YmlConfiguration.DeviceDefinition
{
    public class Register
    {
        public string Name { get; set; }
        
        public string Unit { get; set; }
        
        public int Start { get; set; }
        
        public int Registers { get; set; }
        
        public string DataType { get; set; }
        
        public EnumModbusFunction Function { get; set; }
        
        public string GetStrippedName()
        {
            return Name.StripNonAlphaNumeric();
        }
    }
}
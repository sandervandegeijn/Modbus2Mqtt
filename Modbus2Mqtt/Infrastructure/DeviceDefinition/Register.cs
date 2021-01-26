namespace Modbus2Mqtt.Infrastructure.DeviceDefinition
{
    public class Register
    {
        public string Name { get; set; }
        
        public string Unit { get; set; }
        
        public int Start { get; set; }
        
        public int Registers { get; set; }
        
        public string DataType { get; set; }
        
        public string Function { get; set; }
    }
}
using System.Collections.Generic;

namespace Modbus2Mqtt.Infrastructure.DeviceDefinition
{
    public class DeviceDefition
    {
        public string FriendlyName { get; set; }
        
        public string Endianness { get; set; }
        
        public List<Register> Registers { get; set; }

        public DeviceDefition()
        {
            Registers = new List<Register>();
        }

    }
}
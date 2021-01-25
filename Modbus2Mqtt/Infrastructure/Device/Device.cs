using System.Collections.Generic;

namespace Modbus2Mqtt.Infrastructure.Device
{
    public class Device
    {
        public string FriendlyName { get; set; }
        
        public string Endianness { get; set; }
        
        public List<Register> Registers { get; set; }

        public Device()
        {
            Registers = new List<Register>();
        }

    }
}
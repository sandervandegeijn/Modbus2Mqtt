using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Modbus2Mqtt.Infrastructure.Configuration
{
    public class Configuration
    {
        public string Port { get; set; }
        
        public int Baudrate { get; set; }
        
        public int Databits { get; set; }
        
        public string Parity { get; set; }
        
        public decimal Stopbits { get; set; }
        
        public List<Slave> Slave { get; set; }
        
        public Mqtt Mqtt { get; set; }

        public Configuration()
        {
            Slave = new List<Slave>();
        }
    }
}
using System.Collections.Generic;

namespace Modbus2Mqtt.Infrastructure.Configuration
{
    public class Configuration
    {
        public string Port { get; set; }
        
        public int Baudrate { get; set; }
        
        public int Databits { get; set; }
        
        public string Parity { get; set; }
        
        public decimal Stopbits { get; set; }
        
        public List<Slaves> Slaves { get; set; }
        
        public Mqtt Mqtt { get; set; }

        public Configuration()
        {
            Slaves = new List<Slaves>();
        }
    }
}
using System;
using System.IO.Ports;
using EasyModbus;
using Modbus2Mqtt.Infrastructure.YmlConfiguration;

namespace Modbus2Mqtt.Infrastructure
{
    public static class ModbusclientFactory
    {
        private static ModbusClient ModbusClient { get; set; }

        public static ModbusClient GetModbusClient()
        {
            //Singleton
            if (ModbusClient != null)
            {
                return ModbusClient;
            }

            var config = ConfigurationFactory.GetConfiguration();

            var modbusClient = new ModbusClient(config.Port)
            {
                UnitIdentifier = 1,
                Baudrate = config.Baudrate,
                ConnectionTimeout = 500
            };

            modbusClient.Parity = config.Parity.ToLower() switch
            {
                "none" => Parity.None,
                "even" => Parity.Even,
                "mark" => Parity.Mark,
                "odd" => Parity.Odd,
                "space" => Parity.Space,
                _ => throw new ArgumentOutOfRangeException()
            };

            modbusClient.StopBits = config.Stopbits switch
            {
                1 => StopBits.One,
                2 => StopBits.Two,
                0 => StopBits.None,
                1.5m => StopBits.OnePointFive,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            if (config.Stopbits == 0)
            {
                modbusClient.StopBits = StopBits.None;
            }

            modbusClient.Connect();
            ModbusClient = modbusClient;
            return modbusClient;
        }
    }
}
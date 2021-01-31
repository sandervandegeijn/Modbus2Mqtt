using MediatR;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.Configuration;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.DeviceDefinition;

namespace Modbus2Mqtt.Eventing.ModbusResult
{
    public class ModbusRegisterResult: INotification
    {
        public Register Register { get; set; }
        
        public Slave Slave { get; set; }
        
        public int[] Result { get; set; }
    }
}
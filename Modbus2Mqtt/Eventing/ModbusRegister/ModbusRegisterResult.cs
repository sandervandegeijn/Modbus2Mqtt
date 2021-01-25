using MediatR;
using Modbus2Mqtt.Infrastructure.Configuration;
using Modbus2Mqtt.Infrastructure.Device;

namespace Modbus2Mqtt.Eventing.ModbusRegister
{
    public class ModbusRegisterResult: INotification
    {
        public Register Register { get; set; }
        
        public Slaves Slave { get; set; }
        
        public int[] Result { get; set; }
    }
}
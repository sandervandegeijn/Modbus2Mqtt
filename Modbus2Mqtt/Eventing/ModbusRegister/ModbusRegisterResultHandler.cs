using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EasyModbus;
using MediatR;
using Modbus2Mqtt.Eventing.Mqtt;
using NLog;

namespace Modbus2Mqtt.Eventing.ModbusRegister
{
    public class ModbusRegisterResultHandler : INotificationHandler<ModbusRegisterResult>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly IMediator _mediator;

        public ModbusRegisterResultHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public async Task Handle(ModbusRegisterResult registerResult, CancellationToken cancellationToken)
        {
            if (registerResult.Register.DataType.Equals("float"))
            {
                float parsedResult = 0;
                if (registerResult.Slave.DeviceDefition.Endianness.ToLower().Equals("big-endian"))
                {
                    parsedResult = ModbusClient.ConvertRegistersToFloat(registerResult.Result, ModbusClient.RegisterOrder.HighLow);
                }
                if (registerResult.Slave.DeviceDefition.Endianness.ToLower().Equals("little-endian"))
                {
                    parsedResult = ModbusClient.ConvertRegistersToFloat(registerResult.Result, ModbusClient.RegisterOrder.LowHigh);
                }
                Logger.Info("Result for: " + registerResult.Slave.Name + " register: " + registerResult.Slave.Name +" : " + parsedResult);
                await _mediator.Publish(new OutGoingMessage {Register = registerResult.Register, Slave = registerResult.Slave, Message = parsedResult.ToString(CultureInfo.InvariantCulture)}, cancellationToken);
            }
        }
    }
}
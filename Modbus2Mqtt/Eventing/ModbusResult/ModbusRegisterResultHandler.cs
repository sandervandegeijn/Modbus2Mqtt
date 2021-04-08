using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EasyModbus;
using MediatR;
using Microsoft.Extensions.Logging;
using Modbus2Mqtt.Eventing.Mqtt;

namespace Modbus2Mqtt.Eventing.ModbusResult
{
    public class ModbusRegisterResultHandler : INotificationHandler<ModbusRegisterResultEvent>
    {

        private readonly IMediator _mediator;
        private readonly ILogger<ModbusRegisterResultHandler> _logger;

        public ModbusRegisterResultHandler(IMediator mediator, ILogger<ModbusRegisterResultHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        public async Task Handle(ModbusRegisterResultEvent registerResultEvent, CancellationToken cancellationToken)
        {
            if (registerResultEvent.Register.DataType.Equals("float"))
            {
                double parsedResult = 0;
                if (registerResultEvent.Slave.DeviceDefition.Endianness.ToLower().Equals("big-endian"))
                {
                    parsedResult = ModbusClient.ConvertRegistersToFloat(registerResultEvent.Result, ModbusClient.RegisterOrder.HighLow);
                    parsedResult = Math.Round(parsedResult, registerResultEvent.Register.Decimals);
                }
                if (registerResultEvent.Slave.DeviceDefition.Endianness.ToLower().Equals("little-endian"))
                {
                    parsedResult = ModbusClient.ConvertRegistersToFloat(registerResultEvent.Result, ModbusClient.RegisterOrder.LowHigh);
                    parsedResult = Math.Round(parsedResult, registerResultEvent.Register.Decimals);
                }
                _logger.LogInformation("Result for: " + registerResultEvent.Slave.Name + " register: " + registerResultEvent.Register.Name +" : " + parsedResult);
                await _mediator.Publish(new OutGoingMessageEvent {Register = registerResultEvent.Register, Slave = registerResultEvent.Slave, Message = parsedResult.ToString(CultureInfo.InvariantCulture)}, cancellationToken);
            }
        }

       
    }
}
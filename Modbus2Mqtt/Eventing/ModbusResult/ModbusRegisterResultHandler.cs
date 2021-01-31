﻿using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EasyModbus;
using MediatR;
using Microsoft.Extensions.Logging;
using Modbus2Mqtt.Eventing.Mqtt;

namespace Modbus2Mqtt.Eventing.ModbusResult
{
    public class ModbusRegisterResultHandler : INotificationHandler<ModbusRegisterResult>
    {

        private readonly IMediator _mediator;
        private readonly ILogger<ModbusRegisterResultHandler> _logger;

        public ModbusRegisterResultHandler(IMediator mediator, ILogger<ModbusRegisterResultHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
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
                _logger.LogInformation("Result for: " + registerResult.Slave.Name + " register: " + registerResult.Register.Name +" : " + parsedResult);
                await _mediator.Publish(new OutGoingMessage {Register = registerResult.Register, Slave = registerResult.Slave, Message = parsedResult.ToString(CultureInfo.InvariantCulture)}, cancellationToken);
            }
        }
    }
}
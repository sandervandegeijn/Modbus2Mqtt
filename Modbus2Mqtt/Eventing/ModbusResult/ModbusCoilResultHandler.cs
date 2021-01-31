using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Modbus2Mqtt.Eventing.Mqtt;

namespace Modbus2Mqtt.Eventing.ModbusResult
{
    public class ModbusCoilResultHandler : INotificationHandler<ModbusCoilResult>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ModbusCoilResultHandler> _logger;

        public ModbusCoilResultHandler(IMediator mediator, ILogger<ModbusCoilResultHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(ModbusCoilResult coilResult, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Result for: " + coilResult.Slave.Name + " register: " + coilResult.Slave.Name +" : " + coilResult);
            await _mediator.Publish(new OutGoingMessage {Register = coilResult.Register, Slave = coilResult.Slave, Message = coilResult.ToString()});
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Modbus2Mqtt.Eventing.Mqtt;

namespace Modbus2Mqtt.Eventing.ModbusResult
{
    public class ModbusCoilResultHandler : INotificationHandler<ModbusCoilResultEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ModbusCoilResultHandler> _logger;

        public ModbusCoilResultHandler(IMediator mediator, ILogger<ModbusCoilResultHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(ModbusCoilResultEvent coilResultEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Result for: " + coilResultEvent.Slave.Name + " register: " + coilResultEvent.Slave.Name +" : " + coilResultEvent);
            await _mediator.Publish(new OutGoingMessageEvent {Register = coilResultEvent.Register, Slave = coilResultEvent.Slave, Message = coilResultEvent.ToString()});
        }
    }
}
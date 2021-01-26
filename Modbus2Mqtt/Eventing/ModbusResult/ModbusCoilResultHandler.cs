using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modbus2Mqtt.Eventing.Mqtt;
using NLog;

namespace Modbus2Mqtt.Eventing.ModbusResult
{
    public class ModbusCoilResultHandler : INotificationHandler<ModbusCoilResult>
    {
        private readonly IMediator _mediator;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public ModbusCoilResultHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(ModbusCoilResult coilResult, CancellationToken cancellationToken)
        {
            Logger.Info("Result for: " + coilResult.Slave.Name + " register: " + coilResult.Slave.Name +" : " + coilResult);
            await _mediator.Publish(new OutGoingMessage {Register = coilResult.Register, Slave = coilResult.Slave, Message = coilResult.ToString()});
        }
    }
}
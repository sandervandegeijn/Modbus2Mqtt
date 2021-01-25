using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modbus2Mqtt.Modbus;

namespace Modbus2Mqtt.Eventing.Mqtt
{
    public class IncomingMessageHandler: INotificationHandler<OutGoingMessage>
    {
        private readonly ModbusCommunicator _modbusCommunicator;

        public IncomingMessageHandler(ModbusCommunicator modbusCommunicator)
        {
            _modbusCommunicator = modbusCommunicator;
        }

        public Task Handle(OutGoingMessage message, CancellationToken cancellationToken)
        {
            //throw new System.NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Modbus2Mqtt.Eventing.Mqtt
{
    public class IncomingMessageHandler: INotificationHandler<OutGoingMessage>
    {

        public IncomingMessageHandler()
        {
        }

        public Task Handle(OutGoingMessage message, CancellationToken cancellationToken)
        {
            //throw new System.NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
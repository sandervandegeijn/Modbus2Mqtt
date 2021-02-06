using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Modbus2Mqtt.Eventing.Mqtt
{
    public class IncomingMessageHandler: INotificationHandler<OutGoingMessageEvent>
    {

        public IncomingMessageHandler()
        {
        }

        public Task Handle(OutGoingMessageEvent messageEvent, CancellationToken cancellationToken)
        {
            //throw new System.NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
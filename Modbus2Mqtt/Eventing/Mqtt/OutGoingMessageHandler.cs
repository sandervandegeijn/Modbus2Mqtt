using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MQTTnet.Client;

namespace Modbus2Mqtt.Eventing.Mqtt
{
    public class OutGoingMessageHandler: INotificationHandler<OutGoingMessage>
    {
        private readonly IMqttClient _mqttClient;

        public OutGoingMessageHandler(IMqttClient mqttClient)
        {
            _mqttClient = mqttClient;
        }
        
        public async Task Handle(OutGoingMessage message, CancellationToken cancellationToken)
        {
            await _mqttClient.PublishAsync("modbus2mqtt/get/" + message.Slave.Name + "/" + message.Register.Name, message.Message);
        }
    }
}
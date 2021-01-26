using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MQTTnet.Client;

namespace Modbus2Mqtt.Eventing.Mqtt
{
    public class OutGoingMessageHandler : INotificationHandler<OutGoingMessage>
    {
        private readonly IMqttClient _mqttClient;

        public OutGoingMessageHandler(IMqttClient mqttClient)
        {
            _mqttClient = mqttClient;
        }

        private static string StripNonAlphaNumeric(string input)
        {
            return Regex.Replace(input, @"[^a-zA-Z0-9]+", "-");
        }

        public async Task Handle(OutGoingMessage message, CancellationToken cancellationToken)
        {
            await _mqttClient.PublishAsync("modbus2mqtt/get/" + StripNonAlphaNumeric(message.Slave.Name) + "/" + StripNonAlphaNumeric(message.Register.Name), message.Message);
        }
    }
}
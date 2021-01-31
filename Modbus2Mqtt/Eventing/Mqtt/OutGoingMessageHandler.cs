using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.Configuration;
using MQTTnet.Client;

namespace Modbus2Mqtt.Eventing.Mqtt
{
    public class OutGoingMessageHandler : INotificationHandler<OutGoingMessage>
    {
        private readonly IMqttClient _mqttClient;
        private readonly Configuration _configuration;

        public OutGoingMessageHandler(IMqttClient mqttClient, Configuration configuration)
        {
            _mqttClient = mqttClient;
            _configuration = configuration;
        }

        private static string StripNonAlphaNumeric(string input)
        {
            return Regex.Replace(input, @"[^a-zA-Z0-9]+", "-");
        }

        public async Task Handle(OutGoingMessage message, CancellationToken cancellationToken)
        {
            
            await _mqttClient.PublishAsync(_configuration.Mqtt.MainTopic+ "/get/" + StripNonAlphaNumeric(message.Slave.Name) + "/" + StripNonAlphaNumeric(message.Register.Name), message.Message);
        }
    }
}
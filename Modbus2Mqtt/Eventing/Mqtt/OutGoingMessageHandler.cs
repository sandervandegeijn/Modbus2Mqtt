using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modbus2Mqtt.Infrastructure.Mqtt;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.Configuration;
using MQTTnet.Client;

namespace Modbus2Mqtt.Eventing.Mqtt
{
    public class OutGoingMessageHandler : INotificationHandler<OutGoingMessageEvent>
    {
        private readonly IMqttClient _mqttClient;
        private readonly Configuration _configuration;
        private readonly MqttTopicGenerator _mqttTopicGenerator;

        public OutGoingMessageHandler(IMqttClient mqttClient, Configuration configuration, MqttTopicGenerator mqttTopicGenerator)
        {
            _mqttClient = mqttClient;
            _configuration = configuration;
            _mqttTopicGenerator = mqttTopicGenerator;
        }
        
        public async Task Handle(OutGoingMessageEvent messageEvent, CancellationToken cancellationToken)
        {
            await _mqttClient.PublishAsync(_mqttTopicGenerator.GenerateStateTopic(messageEvent.Slave, messageEvent.Register), messageEvent.Message);
        }
    }
}
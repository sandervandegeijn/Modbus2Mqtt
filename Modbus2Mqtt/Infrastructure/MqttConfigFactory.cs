using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Modbus2Mqtt.Eventing.Mqtt;
using Modbus2Mqtt.Infrastructure.Mqtt;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace Modbus2Mqtt.Infrastructure
{
    public class MqttConfigFactory
    {
        private readonly YmlConfiguration.Configuration.Configuration _configuration;
        private readonly IMediator _mediator;
        private readonly ILogger<MqttConfigFactory> _logger;
        private readonly MqttTopicGenerator _mqttTopicGenerator;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClientOptions _mqttClientOptions;

        private static IMqttClient MqttClient { get; set; }

        public MqttConfigFactory(YmlConfiguration.Configuration.Configuration configuration, IMediator mediator,
            ILogger<MqttConfigFactory> logger, MqttTopicGenerator mqttTopicGenerator)
        {
            _configuration = configuration;
            _mediator = mediator;
            _logger = logger;
            _mqttTopicGenerator = mqttTopicGenerator;
            _mqttFactory = new MqttFactory();

            var options = new MqttClientOptionsBuilder()
                .WithClientId(configuration.Mqtt.ClientId)
                .WithTcpServer(configuration.Mqtt.Server, _configuration.Mqtt.Port)
                .WithCleanSession();

            if (!string.IsNullOrEmpty(configuration.Mqtt.Username) &&
                !string.IsNullOrEmpty(configuration.Mqtt.Password))
            {
                options.WithCredentials(configuration.Mqtt.Username, configuration.Mqtt.Password);
            }

            if (configuration.Mqtt.Tls)
            {
                options.WithTls();
            }

            _mqttClientOptions = options.Build();
        }

        public async Task<IMqttClient> GetMqttClient()
        {
            if (MqttClient == null || !MqttClient.IsConnected)
            {
                return await Connect();
            }
            return MqttClient;
        }

        private async Task<IMqttClient> Connect()
        {
            var mqttClient = _mqttFactory.CreateMqttClient();
            await mqttClient.ConnectAsync(_mqttClientOptions);
            MqttClient = mqttClient;
            await SubscribeToTopics();
            return MqttClient;
        }

        private async Task SubscribeToTopics()
        {
            var topics = _mqttTopicGenerator.GenerateTopicsForIncomingMqttTrafficForAllSlaves();
            foreach (var topic in topics)
            {
                _logger.LogInformation($"Subscribing to: {topic} ");
                
                await MqttClient.SubscribeAsync(topic);
                MqttClient.UseApplicationMessageReceivedHandler(async e =>
                {
                    await _mediator.Publish(new IncomingMessageEvent
                    {
                        Message = e.ApplicationMessage.Payload.ToString(),
                        Topic = e.ApplicationMessage.Topic
                    });
                });
            }
        }
    }
}
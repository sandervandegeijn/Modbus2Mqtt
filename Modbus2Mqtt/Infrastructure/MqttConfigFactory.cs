using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Modbus2Mqtt.Eventing.Mqtt;
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
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClientOptions _mqttClientOptions;

        private static IMqttClient MqttClient { get; set; }

        public MqttConfigFactory(YmlConfiguration.Configuration.Configuration configuration, IMediator mediator, ILogger<MqttConfigFactory> logger)
        {
            _configuration = configuration;
            _mediator = mediator;
            _logger = logger;
            _mqttFactory = new MqttFactory();
            
            var options = new MqttClientOptionsBuilder()
                .WithClientId(configuration.Mqtt.ClientId)
                .WithTcpServer(configuration.Mqtt.Server)
                .WithCleanSession();
            
            if (!string.IsNullOrEmpty(configuration.Mqtt.Username) && !string.IsNullOrEmpty(configuration.Mqtt.Password) )
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
            await SubscribeToTopics();
            MqttClient =  mqttClient;
            return MqttClient;
        }

        private async Task SubscribeToTopics()
        {
            foreach (var slave in _configuration.Slave)
            {
                var registers = (from r in slave.DeviceDefition.Registers
                    where r.Function.StartsWith("write_")
                    select r).ToList();

                foreach (var register in registers)
                {
                    var topic = "modbus2mqtt/set/" + slave.Name + "/" + register.Name;
                    _logger.LogInformation($"Subscribing to: {topic} ");
                    await MqttClient.SubscribeAsync(topic);
                    MqttClient.UseApplicationMessageReceivedHandler(async e =>
                    {
                        await _mediator.Publish(new IncomingMessage
                        {
                            Message = e.ApplicationMessage.Payload.ToString(),
                            Topic = e.ApplicationMessage.Topic
                        });
                    });
                }
            }
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Modbus2Mqtt.Eventing.Mqtt;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using NLog;

namespace Modbus2Mqtt.Infrastructure
{
    public class MqttConfigFactory
    {
        private readonly Configuration.Configuration _configuration;
        private readonly IMediator _mediator;
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClientOptions _mqttClientOptions;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static IMqttClient MqttClient { get; set; }

        public MqttConfigFactory(Configuration.Configuration configuration, IMediator mediator)
        {
            _configuration = configuration;
            _mediator = mediator;
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
                    Logger.Info($"Subscribing to: {topic} ");
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
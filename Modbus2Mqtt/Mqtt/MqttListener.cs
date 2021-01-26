using System.Linq;
using MediatR;
using Modbus2Mqtt.Eventing.Mqtt;
using Modbus2Mqtt.Infrastructure.Configuration;
using MQTTnet.Client;
using NLog;

namespace Modbus2Mqtt.Mqtt
{
    public class MqttListener
    {
        private readonly IMqttClient _mqttClient;
        private readonly Configuration _configuration;
        private readonly IMediator _mediator;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MqttListener(IMqttClient mqttClient, Configuration configuration, IMediator mediator)
        {
            _mqttClient = mqttClient;
            _configuration = configuration;
            _mediator = mediator;
        }

        public void Start()
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
                    _mqttClient.SubscribeAsync(topic);
                    _mqttClient.UseApplicationMessageReceivedHandler(e =>
                    {
                        _mediator.Publish(new IncomingMessage
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
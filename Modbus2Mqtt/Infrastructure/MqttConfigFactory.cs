using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace Modbus2Mqtt.Infrastructure
{
    public class MqttConfigFactory
    {
        private readonly MqttFactory _mqttFactory;
        private readonly IMqttClientOptions _mqttClientOptions;

        public MqttConfigFactory(Configuration.Configuration configuration)
        {
            _mqttFactory = new MqttFactory();
            
            var options = new MqttClientOptionsBuilder()
                .WithClientId("Modbus2Mqtt")
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
            var mqttClient = _mqttFactory.CreateMqttClient();
            await mqttClient.ConnectAsync(_mqttClientOptions);
            return mqttClient;
        }
    }
}
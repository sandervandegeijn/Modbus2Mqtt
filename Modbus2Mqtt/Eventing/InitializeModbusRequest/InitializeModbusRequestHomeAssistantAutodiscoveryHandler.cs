using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modbus2Mqtt.Eventing.NewModbusRequest.HomeAssistantAutodiscovery;
using Modbus2Mqtt.Infrastructure.Modbus;
using Modbus2Mqtt.Infrastructure.Mqtt;
using MQTTnet.Client;

namespace Modbus2Mqtt.Eventing.InitializeModbusRequest
{
    public class InitializeModbusRequestHomeAssistantAutodiscoveryHandler : INotificationHandler<InitializeModbusRequestEvent>
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttTopicGenerator _mqttTopicGenerator;

        public InitializeModbusRequestHomeAssistantAutodiscoveryHandler(IMqttClient mqttClient, MqttTopicGenerator mqttTopicGenerator)
        {
            _mqttClient = mqttClient;
            _mqttTopicGenerator = mqttTopicGenerator;
        }
        
        public async Task Handle(InitializeModbusRequestEvent initializeModbusRequestEvent, CancellationToken cancellationToken)
        {
            var message = AssembleMessage(initializeModbusRequestEvent.ModbusRequest);
            await _mqttClient.PublishAsync(_mqttTopicGenerator.GenerateAvailabilityTopic(), "online", true);
            await _mqttClient.PublishAsync(_mqttTopicGenerator.GenerateHomeAssistantAutodiscoveryTopic(initializeModbusRequestEvent.ModbusRequest.Slave, initializeModbusRequestEvent.ModbusRequest.Register), message, true);
        }

        private string AssembleMessage(ModbusRequest modbusRequest)
        {
            var device = new Device
            {
                Identifiers = $"modbus2mqtt-{modbusRequest.Slave.Name}",
                Name = modbusRequest.Slave.Name,
                Manufacturer = "Modbus2Mqtt",
                Model = modbusRequest.Slave.Name,
                SoftwareVersion = "1.0"
            };

            var message = new Message
            {
                UnitOfMeasurement = modbusRequest.Register.Unit,
                Name = $"{modbusRequest.Slave.Name} {modbusRequest.Register.Name}",
                StateTopic = _mqttTopicGenerator.GenerateStateTopic(modbusRequest.Slave, modbusRequest.Register),
                UniqueId = $"modbus2mqtt-{modbusRequest.Slave.Name}-{modbusRequest.Register.Name}",
                Device = device, 
                Icon = "mdi:leak",
                AvailabilityTopic = _mqttTopicGenerator.GenerateAvailabilityTopic()
            };

            return JsonSerializer.Serialize(message);
        }
        
        
    }
}
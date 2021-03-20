using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modbus2Mqtt.Eventing.NewModbusRequest.HomeAssistantAutodiscovery;
using Modbus2Mqtt.Infrastructure.Modbus;
using Modbus2Mqtt.Infrastructure.Mqtt;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.Configuration;
using MQTTnet.Client;

namespace Modbus2Mqtt.Eventing.InitializeModbusRequest
{
    public class InitializeModbusRequestHomeAssistantAutodiscoveryHandler : INotificationHandler<InitializeModbusRequestEvent>
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttTopicGenerator _mqttTopicGenerator;
        private readonly Configuration _configuration;

        public InitializeModbusRequestHomeAssistantAutodiscoveryHandler(IMqttClient mqttClient, MqttTopicGenerator mqttTopicGenerator, Configuration configuration)
        {
            _mqttClient = mqttClient;
            _mqttTopicGenerator = mqttTopicGenerator;
            _configuration = configuration;
        }
        
        public async Task Handle(InitializeModbusRequestEvent initializeModbusRequestEvent, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(_configuration.HomeassistantAutoDiscoveryPrefix))
            {
                var message = AssembleMessage(initializeModbusRequestEvent.ModbusReadRequest);
                await _mqttClient.PublishAsync(_mqttTopicGenerator.GenerateAvailabilityTopic(), "online", true);
                await _mqttClient.PublishAsync(_mqttTopicGenerator.GenerateHomeAssistantAutodiscoveryTopic(initializeModbusRequestEvent.ModbusReadRequest.Slave, initializeModbusRequestEvent.ModbusReadRequest.Register), message, true);
            }
        }

        private string AssembleMessage(ModbusRequest modbusReadRequest)
        {
            var device = new Device
            {
                Identifiers = $"modbus2mqtt-{modbusReadRequest.Slave.Name}",
                Name = modbusReadRequest.Slave.Name,
                Manufacturer = "Modbus2Mqtt",
                Model = modbusReadRequest.Slave.Name,
                SoftwareVersion = "1.0"
            };

            var message = new Message
            {
                UnitOfMeasurement = modbusReadRequest.Register.Unit,
                Name = $"{modbusReadRequest.Slave.Name} {modbusReadRequest.Register.Name}",
                StateTopic = _mqttTopicGenerator.GenerateStateTopic(modbusReadRequest.Slave, modbusReadRequest.Register),
                UniqueId = $"modbus2mqtt-{modbusReadRequest.Slave.Name}-{modbusReadRequest.Register.Name}",
                Device = device, 
                Icon = "mdi:leak",
                AvailabilityTopic = _mqttTopicGenerator.GenerateAvailabilityTopic()
            };

            return JsonSerializer.Serialize(message);
        }
        
        
    }
}
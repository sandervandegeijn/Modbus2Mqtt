using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modbus2Mqtt.Eventing.NewModbusRequest.HomeAssistantAutodiscovery;
using Modbus2Mqtt.Infrastructure.Modbus;
using Modbus2Mqtt.Infrastructure.Mqtt;
using MQTTnet.Client;

namespace Modbus2Mqtt.Eventing.NewModbusRequest
{
    public class NewModbusRequestHomeAssistantAutodiscoveryHandler : INotificationHandler<NewModbusRequestEvent>
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttTopicGenerator _mqttTopicGenerator;

        public NewModbusRequestHomeAssistantAutodiscoveryHandler(IMqttClient mqttClient, MqttTopicGenerator mqttTopicGenerator)
        {
            _mqttClient = mqttClient;
            _mqttTopicGenerator = mqttTopicGenerator;
        }
        public Task Handle(NewModbusRequestEvent newModbusRequestEvent, CancellationToken cancellationToken)
        {
            var message = AssembleMessage(newModbusRequestEvent.ModbusRequest);
            //await _mqttClient.PublishAsync(_mqttTopicGenerator.GenerateHomeAssistantAutodiscoveryTopic(newModbusRequestEvent.ModbusRequest.Slave, newModbusRequestEvent.ModbusRequest.Register), message, true);
            return Task.CompletedTask;
        }

        private string AssembleMessage(ModbusRequest modbusRequest)
        {
            var device = new Device
            {
                Identifiers = $"modbus2mqtt-{modbusRequest.Slave.Name}",
                Name = modbusRequest.Slave.Name,
            };

            var message = new Message
            {
                UnitOfMeasurement = modbusRequest.Register.Unit,
                Name = modbusRequest.Register.Name,
                StateTopic = _mqttTopicGenerator.GenerateStateTopic(modbusRequest.Slave, modbusRequest.Register),
                UniqueId = $"modbus2mqtt-{modbusRequest.Slave.Name}-{modbusRequest.Register.Name}",
                Device = device
            };

            return JsonSerializer.Serialize(message);
        }
        
        
    }
}
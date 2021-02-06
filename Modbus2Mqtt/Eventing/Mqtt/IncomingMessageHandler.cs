using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Modbus2Mqtt.BackgroundServices;
using Modbus2Mqtt.Infrastructure.Modbus;

namespace Modbus2Mqtt.Eventing.Mqtt
{
    public class IncomingMessageHandler: INotificationHandler<IncomingMessageEvent>
    {
        private readonly ILogger<IncomingMessageEvent> _logger;

        public IncomingMessageHandler(ILogger<IncomingMessageEvent> logger)
        {
            _logger = logger;
        }

        public Task Handle(IncomingMessageEvent messageEvent, CancellationToken cancellationToken)
        {
            
            _logger.LogError("not yet supportaged");
            return Task.CompletedTask;
            
            
            var splitTopic = messageEvent.Topic.Split('/');
            var slaveName = splitTopic[2];
            var registerName = splitTopic[3];
            
            var modbusRequest = new ModbusRequest
            {
                Register = null,
                Slave = null,
                NextExecutionTime = DateTime.Now
            };
            ModbusRequestQueueBackgroundService.Handle(modbusRequest);
            
        }
    }
}
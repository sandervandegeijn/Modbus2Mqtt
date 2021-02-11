using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Modbus2Mqtt.BackgroundServices;
using Modbus2Mqtt.Infrastructure;
using Modbus2Mqtt.Infrastructure.Modbus;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.Configuration;

namespace Modbus2Mqtt.Eventing.Mqtt
{
    public class IncomingMessageHandler : INotificationHandler<IncomingMessageEvent>
    {
        private readonly ILogger<IncomingMessageHandler> _logger;
        private readonly Configuration _configuration;

        public IncomingMessageHandler(ILogger<IncomingMessageHandler> logger, Configuration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task Handle(IncomingMessageEvent messageEvent, CancellationToken cancellationToken)
        {
            var splitTopic = messageEvent.Topic.Split('/');
            var slaveName = splitTopic[2];
            var registerName = splitTopic[3];

            var message = messageEvent.Message;

            var slave = (from s in _configuration.Slave
                where s.GetStrippedName().Equals(slaveName)
                select s).Single();

            var register = (from r in slave.DeviceDefition.Registers
                where r.GetStrippedName().Equals(registerName)
                select r).Single();

            var output = message.GetTfromString<decimal>();
            var modbusRequest = new ModbusRequest
            {
                Register = register,
                Slave = slave,
                Value = output,
                NextExecutionTime = DateTime.Now
            };

            ModbusRequestQueueBackgroundService.Handle(modbusRequest);
            return Task.CompletedTask;
        }
    }
}
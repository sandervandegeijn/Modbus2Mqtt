using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Modbus2Mqtt.Eventing.ModbusRequest;
using Modbus2Mqtt.Infrastructure.Configuration;
using NLog;

namespace Modbus2Mqtt.Modbus
{
    public class TrafficInitiator
    {
        private readonly Configuration _configuration;
        private readonly IMediator _mediator;
        private readonly ModbusRequestHandler _modbusRequestHandler;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public TrafficInitiator(Configuration configuration, IMediator mediator, ModbusRequestHandler modbusRequestHandler)
        {
            _configuration = configuration;
            _mediator = mediator;
            _modbusRequestHandler = modbusRequestHandler;
        }

        public void Start()
        {
            var tasks = new List<Task>();
            foreach (var slave in _configuration.Slave)
            {
                tasks.Add(Task.Factory.StartNew(() => { StartCommunication(slave); }));
            }
        }

        private async void StartCommunication(Slave slave)
        {
            Logger.Info("Starting requests for " + slave.Name);
            while (true)
            {
                foreach (var register in slave.DeviceDefition.Registers)
                {
                    await _modbusRequestHandler.Handle(new ModbusRequest {Slave = slave, Register = register});
                }
                await Task.Delay(slave.PollingInterval);
            }
        }
    }
}
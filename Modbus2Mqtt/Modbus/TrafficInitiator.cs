using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Modbus2Mqtt.Eventing.ModbusRequest;
using Modbus2Mqtt.Infrastructure.Configuration;
using Modbus2Mqtt.Infrastructure.DeviceDefinition;
using Modbus2Mqtt.Modbus.ModbusRequest;
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

        public async void Start()
        {
            var tasks = new List<Task>();
            var wait = 1000;
            foreach (var slave in _configuration.Slave)
            {
                await Task.Delay(wait);
                wait += 1000;
                tasks.Add(Task.Run(async () =>  { await StartCommunication(slave); }));
            }
        }

        private async Task StartCommunication(Slave slave)
        {
            var registers = new List<Register>();
            
            if (!string.IsNullOrEmpty(slave.Include))
            {
                var includedRegisters = slave.Include.Split(";");
                registers = (from r in slave.DeviceDefition.Registers
                    where includedRegisters.Contains(r.Name)
                    select r).ToList();
            }

            if (!string.IsNullOrEmpty(slave.Exclude))
            {
                var includedRegisters = slave.Exclude.Split(";");
                registers = (from r in slave.DeviceDefition.Registers
                    where !includedRegisters.Contains(r.Name)
                    select r).ToList();
            }

            if (string.IsNullOrEmpty(slave.Exclude) && string.IsNullOrEmpty(slave.Include))
            {
                registers = slave.DeviceDefition.Registers;
            }

            Logger.Info("Starting requests for " + slave.Name);
            
            if (registers == null || registers.Count == 0)
            {
                Logger.Error("No registers for device " + slave.Name);
                return;
            }

            var modbusRequests = new List<Eventing.ModbusRequest.ModbusRequest>();
            foreach (var register in registers)
            {
                modbusRequests.Add(new Eventing.ModbusRequest.ModbusRequest {Slave = slave, Register = register});
            }
            
            while (true)
            {
                foreach (var modbusRequest in modbusRequests)
                {
                    _modbusRequestHandler.Handle(modbusRequest);
                }
                await Task.Delay(slave.PollingInterval);
            }
        }
    }
}
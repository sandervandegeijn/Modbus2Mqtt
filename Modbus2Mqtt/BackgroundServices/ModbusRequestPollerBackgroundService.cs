using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Modbus2Mqtt.Eventing.InitializeModbusRequest;
using Modbus2Mqtt.Eventing.NewModbusRequest;
using Modbus2Mqtt.Infrastructure.Modbus;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.Configuration;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.DeviceDefinition;

namespace Modbus2Mqtt.BackgroundServices
{
    public class ModbusRequestPollerBackgroundService : BackgroundService
    {
        private readonly Configuration _configuration;
        private readonly ModbusRequestQueueBackgroundService _modbusRequestQueueBackgroundService;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;


        public ModbusRequestPollerBackgroundService(Configuration configuration,
            ModbusRequestQueueBackgroundService modbusRequestQueueBackgroundService,
            ILogger<ModbusRequestPollerBackgroundService> logger, IMediator mediator)
        {
            _configuration = configuration;
            _modbusRequestQueueBackgroundService = modbusRequestQueueBackgroundService;
            _logger = logger;
            _mediator = mediator;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var slaves = _configuration.Slave;
            _logger.LogDebug($"Number of slaves: {slaves.Count}");

            var modbusRequestsList = new List<ModbusRequest>();
            var wait = 0;

            foreach (var slave in slaves)
            {
                var list = GetModbusRequestListForSlave(slave);
                foreach (var modbusRequest in await list)
                {
                    modbusRequest.NextExecutionTime = DateTime.Now.Add(TimeSpan.FromMilliseconds(wait));
                    modbusRequestsList.Add(modbusRequest);
                }

                wait += 2000;
            }

            if (modbusRequestsList.Count != 0)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var toBeExecuted =
                        (from m in modbusRequestsList
                            where m.NextExecutionTime < DateTime.Now
                            select m).ToList();

                    _logger.LogDebug($"Number of requests to be executed: {toBeExecuted.Count}");

                    foreach (var modbusRequest in toBeExecuted)
                    {
                        ModbusRequestQueueBackgroundService.Handle(modbusRequest);
                        _logger.LogDebug($"Current execution time for modbusrequest {modbusRequest} : {modbusRequest.NextExecutionTime}");
                        modbusRequest.NextExecutionTime = DateTime.Now.AddMilliseconds(modbusRequest.Slave.PollingInterval);
                        _logger.LogDebug($"New execution time for modbusrequest {modbusRequest} : {modbusRequest.NextExecutionTime}");
                    }

                    await Task.Delay(100, stoppingToken);
                }
            }
        }

        private async Task<IEnumerable<ModbusRequest>> GetModbusRequestListForSlave(Slave slave)
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
                registers = (from r in slave.DeviceDefition.Registers
                    where r.Function.ToLower().StartsWith("read_")
                    select r).ToList();
                
                registers = slave.DeviceDefition.Registers;
            }

            if (registers == null || registers.Count == 0)
            {
                _logger.LogError("No registers for device " + slave.Name);
                throw new ArgumentException($"No registers for device {slave.Name} ");
            }

            var modbusRequests = new List<ModbusRequest>();
            
            foreach (var register in registers)
            {
                var modbusRequest = new ModbusRequest {Slave = slave, Register = register};
                var newModbusRequestEvent = new InitializeModbusRequestEvent(modbusRequest);
                await _mediator.Publish(newModbusRequestEvent);
                modbusRequests.Add(modbusRequest);
            }

            return modbusRequests;
        }
    }
}
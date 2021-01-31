using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Modbus2Mqtt.Infrastructure.Configuration;
using Modbus2Mqtt.Infrastructure.DeviceDefinition;
using Modbus2Mqtt.Modbus;
using NLog;

namespace Modbus2Mqtt.BackgroundServices
{
    public class ModbusRequestPollerBackgroundService : BackgroundService
    {
        private readonly Configuration _configuration;
        private readonly ModbusRequestQueueBackgroundService _modbusRequestQueueBackgroundService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public ModbusRequestPollerBackgroundService(Configuration configuration, ModbusRequestQueueBackgroundService modbusRequestQueueBackgroundService)
        {
            _configuration = configuration;
            _modbusRequestQueueBackgroundService = modbusRequestQueueBackgroundService;
        }
        
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var slaves = _configuration.Slave;
            Logger.Debug($"Number of slaves: {slaves.Count}");
            
            
            var modbusRequestsList = new List<ModbusRequest>();
            var wait = 0;
            
            foreach (var slave in slaves)
            {
                var list = GetModbusRequestListForSlave(slave);
                foreach (var modbusRequest in list)
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
                    
                    //Logger.Debug($"Number of requests to be executed: {toBeExecuted.Count}");
                    
                    foreach (var modbusRequest in toBeExecuted)
                    {
                        ModbusRequestQueueBackgroundService.Handle(modbusRequest);
                        //Logger.Debug($"Current execution time for modbusrequest {modbusRequest} : {modbusRequest.NextExecutionTime}");
                        modbusRequest.NextExecutionTime = DateTime.Now.AddMilliseconds(modbusRequest.Slave.PollingInterval);
                        //Logger.Debug($"New execution time for modbusrequest {modbusRequest} : {modbusRequest.NextExecutionTime}");
                    }
                    await Task.Delay(100, stoppingToken);
                }
            }
        }

        private static IEnumerable<ModbusRequest> GetModbusRequestListForSlave(Slave slave)
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

            if (registers == null || registers.Count == 0)
            {
                Logger.Error("No registers for device " + slave.Name);
                throw new ArgumentException($"No registers for device {slave.Name} ");
            }

            var modbusRequests = new List<ModbusRequest>();
            foreach (var register in registers)
            {
                modbusRequests.Add(new ModbusRequest {Slave = slave, Register = register});
            }

            return modbusRequests;
        }
        
    }
}
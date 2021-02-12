using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyModbus;
using EasyModbus.Exceptions;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Modbus2Mqtt.Eventing.ModbusResult;
using Modbus2Mqtt.Infrastructure.Modbus;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.DeviceDefinition;

namespace Modbus2Mqtt.BackgroundServices
{
    public class ModbusRequestQueueBackgroundService : BackgroundService
    {
        private readonly ModbusRequestProxy _modbusRequestProxy;
        private readonly ILogger<ModbusRequestQueueBackgroundService> _logger;
        private static ConcurrentDictionary<Guid, ModbusRequest> _queue;

        public ModbusRequestQueueBackgroundService(ModbusRequestProxy modbusRequestProxy,ILogger<ModbusRequestQueueBackgroundService> logger)
        {
            _modbusRequestProxy = modbusRequestProxy;
            _logger = logger;
            _queue = new ConcurrentDictionary<Guid, ModbusRequest>();
        }

        public static void Handle(ModbusRequest modbusReadRequest)
        {
            var checkForDouble = (from q in _queue
                where q.Value.Slave.Name.Equals(modbusReadRequest.Slave.Name) &&
                      q.Value.Register.Start.Equals(modbusReadRequest.Register.Start)
                select q).Count();

            if (checkForDouble == 0)
            {
                _queue.TryAdd(Guid.NewGuid(), modbusReadRequest);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Items in queue:" + _queue.Count);
                
                if (!_queue.IsEmpty)
                {
                    var modbusRequest = (from m in _queue
                        orderby m.Value.Slave.Priority, m.Value.NextExecutionTime
                        select m).First();

                    await _modbusRequestProxy.SendModbusRequest(modbusRequest.Value, stoppingToken);
                    _queue.TryRemove(modbusRequest);
                }

                if (_queue.IsEmpty)
                {
                    await Task.Delay(100, stoppingToken);
                }
            }
        }
        
        
    }
}
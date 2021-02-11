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
        private static ConcurrentQueue<ModbusRequest> _queue;

        public ModbusRequestQueueBackgroundService(ModbusRequestProxy modbusRequestProxy,ILogger<ModbusRequestQueueBackgroundService> logger)
        {
            _modbusRequestProxy = modbusRequestProxy;
            _logger = logger;
            _queue = new ConcurrentQueue<ModbusRequest>();
        }

        public static void Handle(ModbusRequest modbusReadRequest)
        {
            var checkForDouble = (from q in _queue
                where q.Slave.Name.Equals(modbusReadRequest.Slave.Name) &&
                      q.Register.Start.Equals(modbusReadRequest.Register.Start)
                select q).Count();

            if (checkForDouble == 0)
            {
                _queue.Enqueue(modbusReadRequest);
                _queue.AsParallel().OrderBy(x => x.Slave.Priority);    
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Items in queue:" + _queue.Count);
                _queue.TryDequeue(out var modbusRequest);
                await _modbusRequestProxy.SendModbusRequest(modbusRequest, stoppingToken);

                if (_queue.IsEmpty)
                {
                    await Task.Delay(100, stoppingToken);
                }
            }
        }
        
        
    }
}
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
        private readonly ModbusClient _modbusClient;
        private readonly IMediator _mediator;
        private readonly ILogger<ModbusRequestQueueBackgroundService> _logger;
        private static ConcurrentQueue<ModbusRequest> _queue;

        public ModbusRequestQueueBackgroundService(ModbusClient modbusClient, IMediator mediator, ILogger<ModbusRequestQueueBackgroundService> logger)
        {
            _modbusClient = modbusClient;
            _mediator = mediator;
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
                if (modbusRequest != null)
                {
                    _modbusClient.UnitIdentifier = Convert.ToByte(modbusRequest.Slave.SlaveId);
                    try
                    {
                        //Function code 1
                        if (modbusRequest.Register.Function == EnumModbusFunction.read_coil)
                        {
                            var result = _modbusClient.ReadCoils(modbusRequest.Register.Start, modbusRequest.Register.Registers);
                            await _mediator.Publish(new ModbusCoilResultEvent {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave}, stoppingToken);
                        }

                        //Function code 2
                        if (modbusRequest.Register.Function == EnumModbusFunction.read_discrete_input)
                        {
                            var result = _modbusClient.ReadDiscreteInputs(modbusRequest.Register.Start, modbusRequest.Register.Registers);
                            await _mediator.Publish(new ModbusCoilResultEvent {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave}, stoppingToken);
                        }

                        //Function code 3
                        if (modbusRequest.Register.Function == EnumModbusFunction.read_holding_registers)
                        {
                            var result = _modbusClient.ReadHoldingRegisters(modbusRequest.Register.Start, modbusRequest.Register.Registers);
                            await _mediator.Publish(new ModbusRegisterResultEvent {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave}, stoppingToken);
                        }

                        //Function code 4
                        if (modbusRequest.Register.Function == EnumModbusFunction.read_input_registers)
                        {
                            var result = _modbusClient.ReadInputRegisters(modbusRequest.Register.Start, modbusRequest.Register.Registers);
                            await _mediator.Publish(new ModbusRegisterResultEvent {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave}, stoppingToken);
                        }
                        
                        // //Function code 16
                        // if (modbusRequest.Register.Function.ToLower().Equals("write_multiple_holding_registers"))
                        // {
                        //     HandleModbusWriteRequest(modbusRequest);
                        // }
                        
                    }
                    catch (CRCCheckFailedException)
                    {
                        _logger.LogError($"CRC exception for slave: {modbusRequest.Slave.Name} Register: {modbusRequest.Register.Name}");
                    }
                    catch (TimeoutException)
                    {
                        _logger.LogError($"Timeout for slave: {modbusRequest.Slave.Name} Register: {modbusRequest.Register.Name}");
                    }
                }

                if (_queue.IsEmpty)
                {
                    await Task.Delay(100, stoppingToken);
                }
            }
        }
        
        
    }
}
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using EasyModbus;
using EasyModbus.Exceptions;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Modbus2Mqtt.Eventing.ModbusResult;
using Modbus2Mqtt.Modbus;

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

        public static void Handle(ModbusRequest modbusRequest)
        {
            _queue.Enqueue(modbusRequest);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //Logger.Debug("Items in queue:" + _queue.Count);
                _queue.TryDequeue(out var modbusRequest);
                if (modbusRequest != null)
                {
                    _modbusClient.UnitIdentifier = Convert.ToByte(modbusRequest.Slave.SlaveId);
                    try
                    {
                        //Function code 1
                        if (modbusRequest.Register.Function.ToLower().Equals("read_coil"))
                        {
                            var result = _modbusClient.ReadCoils(modbusRequest.Register.Start, modbusRequest.Register.Registers);
                            await _mediator.Publish(new ModbusCoilResult {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave}, stoppingToken);
                        }

                        //Function code 2
                        if (modbusRequest.Register.Function.ToLower().Equals("read_discrete_input"))
                        {
                            var result = _modbusClient.ReadDiscreteInputs(modbusRequest.Register.Start, modbusRequest.Register.Registers);
                            await _mediator.Publish(new ModbusCoilResult {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave}, stoppingToken);
                        }

                        //Function code 3
                        if (modbusRequest.Register.Function.ToLower().Equals("read_holding_registers"))
                        {
                            var result = _modbusClient.ReadHoldingRegisters(modbusRequest.Register.Start, modbusRequest.Register.Registers);
                            await _mediator.Publish(new ModbusRegisterResult {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave}, stoppingToken);
                        }

                        //Function code 4
                        if (modbusRequest.Register.Function.ToLower().Equals("read_input_registers"))
                        {
                            var result = _modbusClient.ReadInputRegisters(modbusRequest.Register.Start, modbusRequest.Register.Registers);
                            await _mediator.Publish(new ModbusRegisterResult {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave}, stoppingToken);
                        }
                    }
                    catch (CRCCheckFailedException e)
                    {
                        _logger.LogError($"CRC exception for slave: {modbusRequest.Slave.Name} Register: {modbusRequest.Register.Name}");
                    }
                    catch (TimeoutException e)
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
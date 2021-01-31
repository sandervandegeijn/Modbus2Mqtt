using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using EasyModbus;
using EasyModbus.Exceptions;
using MediatR;
using Microsoft.Extensions.Hosting;
using Modbus2Mqtt.Eventing.ModbusResult;
using Modbus2Mqtt.Modbus;
using NLog;

namespace Modbus2Mqtt.BackgroundServices
{
    public class ModbusRequestQueueBackgroundService : BackgroundService
    {
        private readonly ModbusClient _modbusClient;
        private readonly IMediator _mediator;
        private static ConcurrentQueue<ModbusRequest> _queue;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ModbusRequestQueueBackgroundService(ModbusClient modbusClient, IMediator mediator)
        {
            _modbusClient = modbusClient;
            _mediator = mediator;
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
                            await _mediator.Publish(new ModbusCoilResult {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave});
                        }

                        //Function code 2
                        if (modbusRequest.Register.Function.ToLower().Equals("read_discrete_input"))
                        {
                            var result = _modbusClient.ReadDiscreteInputs(modbusRequest.Register.Start, modbusRequest.Register.Registers);
                            await _mediator.Publish(new ModbusCoilResult {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave});
                        }

                        //Function code 3
                        if (modbusRequest.Register.Function.ToLower().Equals("read_holding_registers"))
                        {
                            var result = _modbusClient.ReadHoldingRegisters(modbusRequest.Register.Start, modbusRequest.Register.Registers);
                            await _mediator.Publish(new ModbusRegisterResult {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave});
                        }

                        //Function code 4
                        if (modbusRequest.Register.Function.ToLower().Equals("read_input_registers"))
                        {
                            var result = _modbusClient.ReadInputRegisters(modbusRequest.Register.Start, modbusRequest.Register.Registers);
                            await _mediator.Publish(new ModbusRegisterResult {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave});
                        }
                    }
                    catch (CRCCheckFailedException e)
                    {
                        Logger.Error($"CRC exception for slave: {modbusRequest.Slave.Name} Register: {modbusRequest.Register.Name}");
                        Logger.Error(e);
                    }
                    catch (TimeoutException e)
                    {
                        Logger.Error($"Timeout for slave: {modbusRequest.Slave.Name} Register: {modbusRequest.Register.Name}");
                        Logger.Error(e);
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
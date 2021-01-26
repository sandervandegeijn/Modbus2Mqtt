using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using EasyModbus;
using MediatR;
using Modbus2Mqtt.Eventing.ModbusResult;
using NLog;

namespace Modbus2Mqtt.Eventing.ModbusRequest
{
    public class ModbusRequestHandler
    {
        private readonly ModbusClient _modbusClient;
        private readonly IMediator _mediator;
        private static ConcurrentQueue<ModbusRequest> _queue;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ModbusRequestHandler(ModbusClient modbusClient, IMediator mediator)
        {
            _modbusClient = modbusClient;
            _mediator = mediator;
            _queue = new ConcurrentQueue<ModbusRequest>();
            Start();
        }

        public Task Handle(ModbusRequest modbusRequest)
        {
            _queue.Enqueue(modbusRequest);
            return Task.CompletedTask;
            ;
        }

        private async void Start()
        {
            Logger.Info("Starting Modbus communication");
            while (true)
            {
                _queue.TryDequeue(out var modbusRequest);
                if (modbusRequest != null)
                {
                    _modbusClient.UnitIdentifier = Convert.ToByte(modbusRequest.Slave.SlaveId);
                    
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

                if (_queue.IsEmpty)
                {
                    Logger.Debug("Queue empty waiting 5ms");
                    await Task.Delay(5);
                }
            }
        }
    }
}
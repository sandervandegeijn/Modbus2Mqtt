using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using EasyModbus;
using MediatR;
using Modbus2Mqtt.Eventing.ModbusRegister;
using Modbus2Mqtt.Infrastructure.Configuration;
using Modbus2Mqtt.Infrastructure.Device;
using NLog;

namespace Modbus2Mqtt.Modbus
{
    public class ModbusCommunicator
    {
        private readonly ModbusClient _modbusClient;
        private readonly IMediator _mediator;

        private readonly ConcurrentQueue<Slaves> _queue;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ModbusCommunicator(ModbusClient modbusClient, IMediator mediator)
        {
            _modbusClient = modbusClient;
            _mediator = mediator;
            _queue = new ConcurrentQueue<Slaves>();
            Task.Factory.StartNew(Start);
        }

        public void RequestForSlave(Slaves slave)
        {
            //Todo DTO with time
            _queue.Enqueue(slave);
        }

        public void RequestForRegister(Register register)
        {
            
        }

        private async void Start()
        {
            Logger.Info("Starting Modbus communication");
            while (true)
            {
                _queue.TryDequeue(out var slave);
                if (slave != null)
                {
                    _modbusClient.UnitIdentifier = Convert.ToByte(slave.SlaveId);

                    foreach (var register in slave.Device.Registers)
                    {
                        if (register.Function.ToLower().Equals("read_input_registers"))
                        {
                            var result = _modbusClient.ReadInputRegisters(register.Start, register.Registers);
                            await _mediator.Publish(new ModbusRegisterResult{Result =  result, Register = register, Slave = slave});
                        }
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
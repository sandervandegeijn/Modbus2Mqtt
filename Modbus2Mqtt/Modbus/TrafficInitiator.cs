﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Modbus2Mqtt.Eventing.ModbusRequest;
using Modbus2Mqtt.Infrastructure.Configuration;
using Modbus2Mqtt.Infrastructure.DeviceDefinition;
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

        public void Start()
        {
            var tasks = new List<Task>();
            foreach (var slave in _configuration.Slave)
            {
                tasks.Add(Task.Factory.StartNew(() => { StartCommunication(slave); }));
            }
        }

        private async void StartCommunication(Slave slave)
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
            
            Logger.Info("Starting requests for " + slave.Name);
            while (true)
            {
                if (string.IsNullOrEmpty(slave.Exclude) && string.IsNullOrEmpty(slave.Include))
                {
                    registers = slave.DeviceDefition.Registers;
                }

                if (registers == null || registers.Count == 0)
                {
                    Logger.Error("No registers for device " + slave.Name);
                }
                else
                {

                    foreach (var register in registers)
                    {
                        await _modbusRequestHandler.Handle(new ModbusRequest {Slave = slave, Register = register});
                    }
                }

                await Task.Delay(slave.PollingInterval);
            }
        }
    }
}
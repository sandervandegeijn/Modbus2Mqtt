using System;
using System.Threading;
using System.Threading.Tasks;
using EasyModbus;
using EasyModbus.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Modbus2Mqtt.Eventing.ModbusResult;
using Modbus2Mqtt.Infrastructure.YmlConfiguration.DeviceDefinition;

namespace Modbus2Mqtt.Infrastructure.Modbus
{
    public class ModbusRequestProxy
    {
        private readonly ModbusClient _modbusClient;
        private readonly IMediator _mediator;
        private readonly ILogger<ModbusRequestProxy> _logger;

        public ModbusRequestProxy(ModbusClient modbusClient, IMediator mediator, ILogger<ModbusRequestProxy> logger)
        {
            _modbusClient = modbusClient;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task SendModbusRequest(ModbusRequest modbusRequest, CancellationToken stoppingToken)
        {
            if (modbusRequest != null)
            {
                _modbusClient.UnitIdentifier = Convert.ToByte(modbusRequest.Slave.SlaveId);
                try
                {
                    if (modbusRequest.Register.Function == EnumModbusFunction.read_coil)
                    {
                        var result = _modbusClient.ReadCoils(modbusRequest.Register.Start, modbusRequest.Register.Registers);
                        await _mediator.Publish(new ModbusCoilResultEvent {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave}, stoppingToken);
                    }
                    
                    if (modbusRequest.Register.Function == EnumModbusFunction.read_discrete_input)
                    {
                        var result = _modbusClient.ReadDiscreteInputs(modbusRequest.Register.Start, modbusRequest.Register.Registers);
                        await _mediator.Publish(new ModbusCoilResultEvent {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave}, stoppingToken);
                    }
                    
                    if (modbusRequest.Register.Function == EnumModbusFunction.read_holding_registers)
                    {
                        var result = _modbusClient.ReadHoldingRegisters(modbusRequest.Register.Start, modbusRequest.Register.Registers);
                        await _mediator.Publish(new ModbusRegisterResultEvent {Result = result, Register = modbusRequest.Register, Slave = modbusRequest.Slave}, stoppingToken);
                    }
                    
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
        }
    }
}
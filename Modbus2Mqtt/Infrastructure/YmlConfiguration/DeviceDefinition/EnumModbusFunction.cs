namespace Modbus2Mqtt.Infrastructure.YmlConfiguration.DeviceDefinition
{
    public enum EnumModbusFunction
    {
        read_coil = 1,
        read_discrete_input = 2,
        read_holding_registers = 3,
        read_input_registers = 4,
        write_single_coil = 5,
        write_single_holding_register = 6,
        write_multiple_coils = 15,
        write_multiple_holding_registers = 16
    }
}
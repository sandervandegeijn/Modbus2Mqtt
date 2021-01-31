Modbus2Mqtt

Reading of ModbusRTU data is working, writing from MQTT to modbus still has te be implemented. On Linux however it suffering from a serieus malfunctioning System.IO implementation in .NET core as referenced here: https://github.com/dotnet/runtime/issues/2379 This is resulting in high cpu usage and some stability problems. Have not found an alternative yet....

----

Device definitions are in the DeviceTemplates folder, it is quite easy to add other device types.
Reference the filename in the configuration.yml as shown in the examples.
Dockerfile can be used to build an image for Docker, this works.

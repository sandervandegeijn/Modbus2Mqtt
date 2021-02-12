**Modbus2Mqtt**

This solution has been based on the architecture and ideas of zigbee2mqtt. It uses device templates (see the DeviceTemplates folder) to easilly configure (see configuration.yml.sample) the devices. They will be exposed on mqtt based on the settings and are available through auto discovery in HomeAssistant. It works on both Windows and Linux.

Most devices use RS485 for communication, so you will need an interface adapter that can talk RS485. These are easy to find and really cheap, i.e. https://nl.aliexpress.com/item/32548472327.html?spm=a2g0s.9042311.0.0.27424c4dcTzD43

There is one caveat: the crappy implementation of the System.IO.Ports Serial port in the .NET framework. This does result in errors and relatively high cpu usage, but running it in docker takes care of this somewhat. If you like the solution but are experiencing problems with serial please post here: https://github.com/dotnet/runtime/issues/2379

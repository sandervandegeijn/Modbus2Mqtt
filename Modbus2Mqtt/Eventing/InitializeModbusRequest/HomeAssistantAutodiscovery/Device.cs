using System.Text.Json.Serialization;

namespace Modbus2Mqtt.Eventing.NewModbusRequest.HomeAssistantAutodiscovery
{
    public class Device
    {
        [JsonPropertyName("identifiers")]
        public string Identifiers { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("sw_version")]
        public string SoftwareVersion { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; }
    }
}
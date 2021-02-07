using System.Text.Json.Serialization;

namespace Modbus2Mqtt.Eventing.NewModbusRequest.HomeAssistantAutodiscovery
{
    public class Message
    {
        [JsonPropertyName("unit_of_measurement")]
        public string UnitOfMeasurement { get; set; }
        
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("state_topic")]
        public string StateTopic { get; set; }
        
        [JsonPropertyName("availability_topic")]
        public string AvailabilityTopic { get; set; }
        
        [JsonPropertyName("unique_id")]
        public string UniqueId { get; set; }
        
        [JsonPropertyName("device")]
        public Device Device { get; set; }
    }
}
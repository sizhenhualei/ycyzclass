using System.Text.Json.Serialization;

namespace YcyzClass.Core.Models.Weather;

public class WindInfo
{
    [JsonPropertyName("direction")] public ValueUnitPair Direction { get; set; } = new();
    [JsonPropertyName("speed")] public ValueUnitPair Speed { get; set; } = new();
}
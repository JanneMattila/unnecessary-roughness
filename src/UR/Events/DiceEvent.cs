using System.Text.Json.Serialization;

namespace UR.Events;

public class DiceEvent : Event
{
    [JsonPropertyName("dices")]
    public List<string> Dices { get; set; } = new();
}

using System.Text.Json.Serialization;
using UR.Data;

namespace UR.Events;

public class PlacePlayersEvent : Event
{
    [JsonPropertyName("players")]
    public List<Player> Players { get; set; } = new();
}

using System.Text.Json.Serialization;
using UR.Data;

namespace UR.Events;

public class MovePlayerEvent : Event
{
    [JsonPropertyName("player_id")]
    public string PlayerID { get; set; } = String.Empty;

    [JsonPropertyName("moves")]
    public List<BoardPosition> Moves { get; set; } = new();
}

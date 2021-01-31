using System.Text.Json.Serialization;
using UR.Data;

namespace UR.Events
{
    public class GameDataEvent : Event
    {
        [JsonPropertyName("game")]
        public Game Game { get; set; } = new();
    }
}

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UR.Events
{
    public class Event
    {
        [JsonPropertyName("id")]
        public Guid ID { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("initiatedBy")]
        public Guid InitiatedBy { get; set; }

        public string ToJson()
        {
            this.Type = this.GetType().Name;
            return JsonSerializer.Serialize(this);
        }

        public static List<Event> FromJsonToEventList(string json)
        {
            var list = new List<Event>();
            var jsonDocument = JsonDocument.Parse(json);
            foreach (var element in jsonDocument.RootElement.EnumerateArray())
            {
                var evt = ConvertJsonToEvent(element);

                list.Add(evt);
            }
            return list;
        }

        private static Event ConvertJsonToEvent(JsonElement element)
        {
            var type = element.GetProperty("type").GetString();
            var elementJson = element.GetRawText();

            return ConvertToEvent(type, elementJson);
        }

        public static Event ConvertJsonToEvent(string json)
        {
            var jsonDocument = JsonDocument.Parse(json);

            var type = jsonDocument.RootElement.GetProperty("type").GetString();
            var elementJson = jsonDocument.RootElement.GetRawText();

            return ConvertToEvent(type, elementJson);
        }

        private static Event ConvertToEvent(string? type, string elementJson)
        {
            Event? evt = type switch
            {
                nameof(DiceEvent) => JsonSerializer.Deserialize<DiceEvent>(elementJson),
                nameof(GameDataEvent) => JsonSerializer.Deserialize<GameDataEvent>(elementJson),
                _ => throw new Exception($"Unsupported json type {type}")
            };

            if (evt == null)
            {
                throw new Exception($"Cannot parse json type {type}");
            }

            return evt;
        }
    }
}

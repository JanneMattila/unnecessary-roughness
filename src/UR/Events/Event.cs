using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;

namespace UR.Events
{
    [XmlInclude(typeof(DiceEvent))]
    [XmlInclude(typeof(GameDataEvent))]
    public class Event
    {
        public Guid ID { get; set; }

        public Guid InitiatedBy { get; set; }

        private static readonly XmlSerializer s_serializer = new(typeof(Event));
        private static readonly XmlSerializer s_serializerList = new(typeof(List<Event>));

        public Event()
        {
        }

        public string ToXml()
        {
            using var writer = new StringWriter();
            s_serializer.Serialize(writer, this);
            writer.Flush();
            return writer.ToString();
        }

        [return: MaybeNull]
        public static T FromXmlToEvent<T>(string xml) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return serializer.Deserialize(reader) as T;
        }

        [return: MaybeNull]
        public static List<Event> FromXmlToEventList(string xml)
        {
            using var reader = new StringReader(xml);
            return s_serializerList.Deserialize(reader) as List<Event>;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace UR.Events
{
    [XmlInclude(typeof(DiceEvent))]
    public class Event
    {
        public Guid ID { get; set; }

        public Guid InitiatedBy { get; set; }

        private static readonly XmlSerializer s_serializer = new XmlSerializer(typeof(Event));
        private static readonly XmlSerializer s_serializerList = new XmlSerializer(typeof(List<Event>));

        public string ToXml()
        {
            using var writer = new StringWriter();
            s_serializer.Serialize(writer, this);
            writer.Flush();
            return writer.ToString();
        }

        static public T FromXmlToEvent<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader);
        }

        static public List<Event> FromXmlToEventList(string xml)
        {
            using var reader = new StringReader(xml);
            return s_serializerList.Deserialize(reader) as List<Event>;
        }
    }
}
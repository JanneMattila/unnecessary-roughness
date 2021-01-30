using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UR.Events;

namespace UR
{
    public class EventStoreBackend : IEventStore
    {
        [AllowNull]
        public Func<string, string,Task> HttpPutEventAsync;

        [AllowNull]
        public Func<string, Task<string>> HttpGetEventsAsync;

        public async Task AppendEventAsync(string id, Event e)
        {
            var xml = e.ToXml();
            await HttpPutEventAsync(id, xml);
        }

        public async Task<List<Event>> GetEventsAsync(string id)
        {
            var xml = await HttpGetEventsAsync(id);
            return Event.FromXmlToEventList(xml) ?? new List<Event>();
        }
    }
}

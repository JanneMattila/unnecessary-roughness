using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UR.Events;

namespace UR
{
    public class EventStoreBackend : IEventStore
    {
        public Func<string, string,Task> HttpPutEventAsync;
        public Func<string, Task<string>> HttpGetEventsAsync;

        public async Task AppendEventAsync(string id, Event e)
        {
            var xml = e.ToXml();
            await HttpPutEventAsync(id, xml);
        }

        public async Task<List<Event>> GetEventsAsync(string id)
        {
            var xml = await HttpGetEventsAsync(id);
            return Event.FromXmlToEventList(xml);
        }
    }
}

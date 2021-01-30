using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UR.Events;
using UR.Server.Hubs;

namespace UR.Server.Controllers
{
    [Route("api/[controller]")]
    public class EventsController : Controller
    {
        private static bool s_loaded;
        private static List<Event> s_events = new List<Event>();

        private readonly IHubContext<GameHub> _hub;

        public EventsController(IHubContext<GameHub> hub)
        {
            _hub = hub;

            if (!s_loaded)
            {
                s_loaded = true;

#if DEBUG
                // Restore existing
                if (System.IO.File.Exists("eventstore.xml"))
                {
                    var serializer = new XmlSerializer(typeof(List<Event>));
                    using var reader = System.IO.File.OpenRead("eventstore.xml");
                    s_events = serializer.Deserialize(reader) as List<Event> ?? new List<Event>();
                }

                // Override to be blank
                //var serializer = new XmlSerializer(s_events.GetType());
                //using (var writer = new StringWriter())
                //{
                //    serializer.Serialize(writer, s_events);
                //}
#endif
            }
        }

        [HttpGet("{id}")]
        public async Task<string> GetEvents(string id)
        {
            var serializer = new XmlSerializer(s_events.GetType());
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, s_events);
                await writer.FlushAsync();
                return writer.ToString();
            }
        }

        [HttpPut("{id}")]
        public async Task AppendEvent(string id, [FromBody] Event e)
        {
            s_events.Add(e);
#if DEBUG
            var serializer = new XmlSerializer(s_events.GetType());
            using (var writer = System.IO.File.CreateText("eventstore.xml"))
            {
                serializer.Serialize(writer, s_events);
            }
#endif
            await _hub.Clients.All.SendAsync(HubConstants.AppendEventMethod, e);
        }

        [HttpDelete]
        public void DeleteState()
        {
            s_events.Clear();
#if DEBUG
            var serializer = new XmlSerializer(s_events.GetType());
            using var writer = System.IO.File.CreateText("eventstore.xml");
            serializer.Serialize(writer, s_events);
#endif
        }
    }
}

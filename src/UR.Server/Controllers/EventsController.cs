using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UR.Events;
using UR.Server.Hubs;

namespace UR.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
#if DEBUG
    private const string EVENTSTORE_FILENAME = "eventstore.json";
#endif

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
            if (System.IO.File.Exists(EVENTSTORE_FILENAME))
            {
                var json = System.IO.File.ReadAllText(EVENTSTORE_FILENAME);
                s_events = Event.FromJsonToEventList(json);
            }
#endif
        }
    }

    [HttpGet("{id}")]
    public string GetEvents(string id)
    {
        return JsonSerializer.Serialize(s_events);
    }

    [HttpPut("{id}")]
    public async Task AppendEvent(string id)
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();

        var evt = Event.ConvertJsonToEvent(json);
        s_events.Add(evt);
#if DEBUG
        var eventsJson = JsonSerializer.Serialize(s_events, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
        await System.IO.File.WriteAllTextAsync(EVENTSTORE_FILENAME, eventsJson);
#endif
        await _hub.Clients.All.SendAsync(HubConstants.AppendEventMethod, evt);
    }

    [HttpDelete]
    public async Task DeleteState()
    {
        s_events.Clear();
#if DEBUG
        var eventsJson = JsonSerializer.Serialize(s_events);
        await System.IO.File.WriteAllTextAsync(EVENTSTORE_FILENAME, eventsJson);
#else
        await Task.CompletedTask;
#endif
    }
}

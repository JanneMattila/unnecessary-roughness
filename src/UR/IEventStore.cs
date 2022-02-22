using UR.Events;

namespace UR;

public interface IEventStore
{
    Task<List<Event>> GetEventsAsync(string id);

    Task AppendEventAsync(string id, Event e);
}

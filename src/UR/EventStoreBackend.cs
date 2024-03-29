﻿using System.Diagnostics.CodeAnalysis;
using UR.Events;

namespace UR;

public class EventStoreBackend : IEventStore
{
    [AllowNull]
    public Func<string, string, Task> HttpPutEventAsync;

    [AllowNull]
    public Func<string, Task<string>> HttpGetEventsAsync;

    public async Task AppendEventAsync(string id, Event e)
    {
        var json = e.ToJson();
        await HttpPutEventAsync(id, json);
    }

    public async Task<List<Event>> GetEventsAsync(string id)
    {
        var xml = await HttpGetEventsAsync(id);
        return Event.FromJsonToEventList(xml) ?? new List<Event>();
    }
}

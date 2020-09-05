using System;
using System.Threading.Tasks;
using UR.Events;

namespace UR
{
    public class RandomizerBackend : IRandomizer
    {
        public Func<string, string, int, Task<string>> FetchDicesAsync;

        public async Task<DiceEvent> GetDicesAsync(string id, string type, int count)
        {
            var xml = await FetchDicesAsync(id, type, count);
            return Event.FromXmlToEvent<DiceEvent>(xml);
        }
    }
}

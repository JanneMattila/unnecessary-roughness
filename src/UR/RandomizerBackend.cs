using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using UR.Events;

namespace UR
{
    public class RandomizerBackend : IRandomizer
    {
        [AllowNull]
        public Func<string, string, int, Task<string>> FetchDicesAsync;

        public async Task<DiceEvent> GetDicesAsync(string id, string type, int count)
        {
            var xml = await FetchDicesAsync(id, type, count);
            var evt = Event.FromXmlToEvent<DiceEvent>(xml);
            if (evt == null) throw new Exception("Cannot convert data to dice roll");
            return evt;
        }
    }
}

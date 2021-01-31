using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
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
            var json = await FetchDicesAsync(id, type, count);
            var evt = JsonSerializer.Deserialize<DiceEvent>(json);
            if (evt == null) throw new Exception("Cannot convert data to dice roll");
            return evt;
        }
    }
}

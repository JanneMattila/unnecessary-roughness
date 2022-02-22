using UR.Events;

namespace UR;

public interface IRandomizer
{
    Task<DiceEvent> GetDicesAsync(string id, string type, int count);
}

using System.Collections.Generic;

namespace UR.Events
{
    public class DiceEvent : Event
    {
        public List<string> Dices { get; set; } = new();
    }
}

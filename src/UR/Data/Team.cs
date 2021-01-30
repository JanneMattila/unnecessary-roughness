using System.Collections.Generic;

namespace UR.Data
{
    public class Team
    {
        public Coach Coach { get; set; } = new();

        public string ID { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public List<Player> Players { get; set; } = new();

        public void ResetMovement()
        {
            Players.ForEach(p => p.MovementLeft = p.MovementAllowance);
        }
    }
}

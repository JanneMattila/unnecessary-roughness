using System;
using System.Collections.Generic;

namespace UR.Data
{
    public class Team
    {
        public Coach Coach { get; set; }

        public string ID { get; set; }

        public string Image { get; set; }

        public string Name { get; set; }

        public List<Player> Players { get; set; }

        public Team()
        {
            Players = new List<Player>();
        }

        public void ResetMovement()
        {
            Players.ForEach(p => p.MovementLeft = p.MovementAllowance);
        }
    }
}

using System.Collections.Generic;

namespace UR.Data
{
    public class Player
    {
        public string ID { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public string Position { get; set; }

        public BoardPosition BoardPosition { get; set; }

        public int Rotation { get; set; }

        public int MovementLeft { get; set; }

        public int MovementAllowance { get; set; }

        public int Strength { get; set; }

        public int Agility { get; set; }

        public int ArmourValue { get; set; }
        
        public int Cost { get; set; }

        public List<string> Skills { get; set; }

        public string Team { get; set; }

        public Player()
        {
            Skills = new List<string>();
            BoardPosition = new BoardPosition();
        }
    }
}

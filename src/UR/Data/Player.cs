using System.Collections.Generic;

namespace UR.Data
{
    public class Player
    {
        public string ID { get; set; } = string.Empty;

        public string Number { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;

        public BoardPosition BoardPosition { get; set; } = new();

        public int Rotation { get; set; }

        public int MovementLeft { get; set; }

        public int MovementAllowance { get; set; }

        public int Strength { get; set; }

        public int Agility { get; set; }

        public int ArmourValue { get; set; }
        
        public int Cost { get; set; }

        public List<string> Skills { get; set; } = new();

        public string Team { get; set; } = string.Empty;

        public static Player Empty
        {
            get
            {
                return new Player();
            }
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(ID);
        }
    }
}

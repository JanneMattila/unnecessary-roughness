using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UR.Data
{
    public class Game
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }

        public int Turn { get; set; }

        public Team HomeTeam { get; set; }

        public Team VisitorTeam { get; set; }

        public Ball Ball { get; set; }

        public Player? SelectedPlayer { get; set; }

        public List<BoardPosition> AvailableMoves { get; set; }

        public List<BoardPosition> AvailableExtraMoves { get; set; }

        public List<BoardPosition> SelectedMoves { get; set; }

        public Game()
        {
            ID = string.Empty;
            HomeTeam = new Team();
            VisitorTeam = new Team();
            Ball = new Ball();
            Turn = 1;
            AvailableMoves = new List<BoardPosition>();
            AvailableExtraMoves = new List<BoardPosition>();
            SelectedMoves = new List<BoardPosition>();
        }

        public void SetPlayersToDefaultLocations()
        {
            var x = 2;
            foreach (var player in HomeTeam.Players)
            {
                player.Rotation = 0;
                player.BoardPosition = new BoardPosition()
                {
                    X = x++,
                    Y = 13
                };
            }

            x = 2;
            foreach (var player in VisitorTeam.Players)
            {
                player.Rotation = 270; // Look down
                player.BoardPosition = new BoardPosition()
                {
                    X = x++,
                    Y = 12
                };
            }
        }
    }
}

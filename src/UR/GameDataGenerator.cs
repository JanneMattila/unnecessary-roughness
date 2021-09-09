using System;
using UR.Data;

namespace UR
{
    public class GameDataGenerator
    {
        public Game NewGame()
        {
            var homeTeam = CreateTeam("Best blue team", "blue", true);
            var visitorTeam = CreateTeam("Best red team", "red", false);

            var game = new Game()
            {
                ID = Guid.NewGuid().ToString("D"),
                HomeTeam = homeTeam,
                VisitorTeam = visitorTeam
            };
            return game;
        }

        private static Team CreateTeam(string name, string color, bool upperHalf)
        {
            var team = new Team()
            {
                ID = Guid.NewGuid().ToString("D"),
                Name = name,
                Image = color,
                Coach = new Coach()
                {
                    ID = Guid.NewGuid()
                }
            };

            var y = upperHalf ? 10 : 39;
            for (var i = 0; i < 4; i++)
            {
                AddPlayer(team, i + 1, 3 + 6 * i, y, "goalie");
            }

            y = upperHalf ? 15 : 34;
            for (var i = 0; i < 3; i++)
            {
                AddPlayer(team, i + 5, 5 + 7 * i, y, "fullback");
            }

            y = upperHalf ? 20 : 29;
            for (var i = 0; i < 5; i++)
            {
                AddPlayer(team, i + 8, 4 + 4 * i, y, "halfback");
            }

            y = upperHalf ? 24 : 25;
            for (var i = 0; i < 15; i++)
            {
                AddPlayer(team, i + 12, 5 + i, y, "forward");
            }
            return team;
        }

        private static void AddPlayer(Team team, int i, int x, int y, string position)
        {
            team.Players.Add(new Player()
            {
                ID = i.ToString(),
                Name = $"Player {i} {position} - {team.Name}",
                Position = position,
                BoardPosition = new BoardPosition()
                {
                    X = x,
                    Y = y
                },
                Team = team.ID
            });
        }
    }
}

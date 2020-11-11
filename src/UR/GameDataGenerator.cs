using System;
using UR.Data;

namespace UR
{
    public class GameDataGenerator
    {
        public Game NewGame()
        {
            var homeTeam = CreateTeam("Best team", 20);
            var visitorTeam = CreateTeam("Best team", 30);

            var game = new Game()
            {
                ID = Guid.NewGuid().ToString("B"),
                HomeTeam = homeTeam,
                VisitorTeam = visitorTeam
            };
            return game;
        }

        private static Team CreateTeam(string name, int row)
        {
            var team = new Team()
            {
                ID = Guid.NewGuid().ToString("B"),
                Name = name,
                Coach = new Coach()
                {
                    ID = Guid.NewGuid()
                }
            };

            for (int i = 0; i < 27; i++)
            {
                team.Players.Add(new Player()
                {
                    ID = i.ToString(),
                    BoardPosition = new BoardPosition()
                    {
                        X = 10 + i,
                        Y = row
                    }
                });
            }
            return team;
        }
    }
}

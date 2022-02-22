using System.Text.Json.Serialization;

namespace UR.Data;

public class Game
{
    [JsonPropertyName("id")]
    public string ID { get; set; } = string.Empty;

    public int Turn { get; set; } = 1;

    public Team HomeTeam { get; set; } = new();

    public Team VisitorTeam { get; set; } = new();

    public Ball Ball { get; set; } = new();

    public Player? SelectedPlayer { get; set; }

    public List<BoardPosition> AvailableMoves { get; set; } = new();

    public List<BoardPosition> SelectedMoves { get; set; } = new();

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

using System.Text.Json.Serialization;

namespace UR.Data;

public class BoardPosition
{
    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    public BoardPosition()
    {
        X = -1;
        Y = -1;
    }

    public override bool Equals(object? obj)
    {
        if (obj is BoardPosition other)
        {
            if (X == other.X && Y == other.Y)
            {
                return true;
            }
        }

        return false;
    }

    public override int GetHashCode()
    {
        var hashCode = 1861411795;
        hashCode = hashCode * -1521134295 + X.GetHashCode();
        hashCode = hashCode * -1521134295 + Y.GetHashCode();
        return hashCode;
    }

    public static bool operator ==(BoardPosition left, BoardPosition right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BoardPosition left, BoardPosition right)
    {
        return !(left == right);
    }

    public BoardPosition Clone() => new BoardPosition() { X = X, Y = Y };
}

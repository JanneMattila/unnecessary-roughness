namespace UR.Data;

public class Ball
{
    public BoardPosition BoardPosition { get; set; }

    public Ball()
    {
        // Default to be out of bounds
        BoardPosition = new BoardPosition()
        {
            X = -1,
            Y = -1
        };
    }
}

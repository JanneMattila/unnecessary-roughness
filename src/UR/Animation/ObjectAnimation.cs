using UR.Data;

namespace UR.Animation;

public class ObjectAnimation
{
    public string ID { get; set; } = string.Empty;
    public int Index { get; set; }

    public BoardPosition Position { get; set; } = new();

    public int Rotation { get; set; }

    public double StartTime { get; set; }

    public List<AnimationMove> Animations { get; set; } = new();
}

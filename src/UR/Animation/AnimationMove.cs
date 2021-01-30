using UR.Data;

namespace UR.Animation
{
    public class AnimationMove
    {
        public BoardPosition Source { get; set; } = new();

        public BoardPosition Target { get; set; } = new();

        public int Rotation { get; set; }

        public double TimeElapsed { get; set; }

        public double AnimationTime { get; set; }
    }
}

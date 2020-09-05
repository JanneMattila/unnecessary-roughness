using UR.Data;

namespace UR.Animation
{
    public class AnimationMove
    {
        public BoardPosition Source { get; set; }

        public BoardPosition Target { get; set; }

        public int Rotation { get; set; }

        public double TimeElapsed { get; set; }

        public double AnimationTime { get; set; }
    }
}

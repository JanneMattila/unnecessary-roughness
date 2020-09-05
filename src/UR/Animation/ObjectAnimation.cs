using System.Collections.Generic;
using UR.Data;

namespace UR.Animation
{
    public class ObjectAnimation
    {
        public string ID { get; set; }
        public int Index { get; set; }

        public BoardPosition Position { get; set; }

        public int Rotation { get; set; }

        public double StartTime { get; set; }

        public List<AnimationMove> Animations { get; set; }

        public ObjectAnimation()
        {
            Animations = new List<AnimationMove>();
        }
    }
}

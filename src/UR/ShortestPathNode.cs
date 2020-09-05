using System.Collections.Generic;
using UR.Data;

namespace UR
{
    public class ShortestPathNode
    {
        public BoardPosition Position { get; set; }
        public List<BoardPosition> Path { get; set; }

        public ShortestPathNode()
        {
            Path = new List<BoardPosition>();
        }
    }
}

using System.Collections.Generic;
using UR.Data;

namespace UR
{
    public class ShortestPathNode
    {
        public BoardPosition Position { get; set; } = new();
        public List<BoardPosition> Path { get; set; } = new();
    }
}

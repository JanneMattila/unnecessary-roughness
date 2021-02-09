using System;
using System.Linq;
using UR.Data;
using UR.Events;

namespace UR
{
    public static class GameBoard
    {
        public const int BoardWidth = 25;
        public const int BoardHeight = 50;
        public const int BoardTopHalf = 24;
        public const int BoardBottomHalf = 25;

        public static bool Scatter(DiceEvent d8event, int length, BoardPosition position)
        {
            var d8 = Convert.ToInt32(d8event.Dices.First());
            for (var i = 0; i < length; i++)
            {
                switch (d8)
                {
                    case 1:
                        position.X--;
                        position.Y--;
                        break;
                    case 2:
                        position.Y--;
                        break;
                    case 3:
                        position.X++;
                        position.Y--;
                        break;
                    case 4:
                        position.X--;
                        break;
                    case 5:
                        position.X++;
                        break;
                    case 6:
                        position.X--;
                        position.Y++;
                        break;
                    case 7:
                        position.Y++;
                        break;
                    case 8:
                        position.X++;
                        position.Y++;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if (position.X < 0 || position.X >= BoardWidth ||
                    position.Y < 0 || position.Y >= BoardHeight)
                {
                    // Out of bounds
                    return true;
                }
            }

            return false;
        }

        public static bool ThrowIn(DiceEvent d6event, BoardPosition position)
        {
            var d6 = d6event.Dices.Select(d => Convert.ToInt32(d)).ToList();
            var throwIn = d6[0];
            var distance = d6[1] + d6[2];

            var dx = 0;
            var dy = 0;

            if ((position.X < 0 && position.Y < 0) ||
                (position.X >= BoardWidth && position.Y < 0))
            {
                // Out from top corners. Force it to be end zone.
                position.Y = 0;
            }
            else if ((position.X < 0 && position.Y >= BoardHeight) ||
                (position.X >= BoardWidth && position.Y >= BoardHeight))
            {
                // Out from bottom corners. Force it to be end zone.
                position.Y = BoardHeight - 1;
            }

            switch (throwIn)
            {
                case 1:
                case 2:
                    if (position.X < 0)
                    {
                        // Out from left side
                        dx = 1;
                        dy = -1;
                    }
                    else if (position.X >= BoardWidth)
                    {
                        // Out from right side
                        dx = -1;
                        dy = 1;
                    }
                    else if (position.Y < 0)
                    {
                        // Out from top side
                        dx = 1;
                        dy = 1;
                    }
                    else if (position.Y >= BoardHeight)
                    {
                        // Out from bottom side
                        dx = -1;
                        dy = -1;
                    }
                    break;
                case 3:
                case 4:
                    if (position.X < 0)
                    {
                        // Out from left side
                        dx = 1;
                    }
                    else if (position.X >= BoardWidth)
                    {
                        // Out from right side
                        dx = -1;
                    }
                    else if (position.Y < 0)
                    {
                        // Out from top side
                        dy = 1;
                    }
                    else if (position.Y >= BoardHeight)
                    {
                        // Out from bottom side
                        dy = -1;
                    }
                    break;
                case 5:
                case 6:
                    if (position.X < 0)
                    {
                        // Out from left side
                        dx = 1;
                        dy = 1;
                    }
                    else if (position.X >= BoardWidth)
                    {
                        // Out from right side
                        dx = -1;
                        dy = -1;
                    }
                    else if (position.Y < 0)
                    {
                        // Out from top side
                        dx = -1;
                        dy = 1;
                    }
                    else if (position.Y >= BoardHeight)
                    {
                        // Out from bottom side
                        dx = 1;
                        dy = -1;
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }

            for (var i = 0; i < distance; i++)
            {
                position.X += dx;
                position.Y += dy;

                if (position.X < 0 || position.X >= BoardWidth ||
                    position.Y < 0 || position.Y >= BoardHeight)
                {
                    // Out of bounds
                    return true;
                }
            }

            return false;
        }

        public static double CalculateDistanceBetweenPositions(BoardPosition source, BoardPosition target)
        {
            var dx = source.X - target.X;
            var dy = source.Y - target.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static int CalculateRotation(BoardPosition source, BoardPosition target)
        {
            var dx = target.X - source.X;
            var dy = target.Y - source.Y;

            return Convert.ToInt32(Math.Atan2(dy, dx) * 180.0 / Math.PI) + 90;
        }
    }
}

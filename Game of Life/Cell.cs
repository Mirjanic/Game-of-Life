using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game_of_Life
{
    class Cell
    {
        Point location;
        List<Cell> neighbors;
        public Cell(int x, int y)
        {
            location = new Point(x, y);
            neighbors = new List<Cell>();
            int[] d = { -1, 0, 1 };
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (i != 1 || j != 1) Move(i, j);
        }
        public Cell(Point p)
        {
            location = p;
            neighbors = new List<Cell>();
            int[] d = { -1, 0, 1 };
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (i != 1 || j != 1) Move(i, j);
        }
        public Cell Move(int x, int y)
        {
            return new Cell(location.X + x, location.Y + y);
        }
        public List<Cell> Neighbors { get { return neighbors; } }
        public Point Location { get { return location; } }
        public static IEqualityComparer<Cell> Comparer() { return new EqualityComparer(); }
        class EqualityComparer : IEqualityComparer<Cell>
        {
            public bool Equals(Cell x, Cell y)
            {
                return x.location == y.location;
            }

            public int GetHashCode(Cell obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}

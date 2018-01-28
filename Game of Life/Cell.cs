using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game_of_Life
{
    class Cell : IEquatable<Cell>
    {
        Point location;
        public Cell(int x, int y)
        {
            location = new Point(x, y);
        }
        public Cell(Point p)
        {
            location = p;
        }
        public Cell Move(int x, int y)
        {
            return new Cell(location.X + x, location.Y + y);
        }
        public bool Equals(Cell other)
        {
            return location == other.location;
        }
        public override int GetHashCode()
        {
            return location.GetHashCode();
        }
        public List<Cell> Neighbors
        {
            get
            {
                List<Cell> neighbors;
                neighbors = new List<Cell>();
                for (int i = -1; i < 2; i++)
                    for (int j = -1; j < 2; j++)
                        if (i != 0 || j != 0) neighbors.Add(Move(i, j));
                return neighbors;
            }
        }
        public Point Location { get { return location; } }
        public static implicit operator Point(Cell c) { return c.location; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Primitives;

namespace RogueTutorial.Map
{
    internal class Rectangle
    {
        public int X1 { get; set; }
        public int X2 { get; set; }
        public int Y1 { get; set; }
        public int Y2 { get; set; }

        public Rectangle(int x, int y, int width, int height)
        {
            X1 = x;
            X2 = x + width;
            Y1 = y;
            Y2 = y + height;
        }

        public bool Intersect(ref Rectangle other)
        {
            return X1 <= other.X2 && X2 >= other.X1 && Y1 <= other.Y2 && Y2 >= other.Y1;
        }

        public Point Center()
        {
            return new Point((X1 + X2) / 2, (Y1 + Y2) / 2);
        }
    }
}

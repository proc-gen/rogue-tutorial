using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueTutorial.Interfaces;
using RogueTutorial.Utils;
using SadRogue.Primitives;

namespace RogueTutorial.Map
{
    public class Rectangle : ISaveable
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

        public StringBuilder Save(StringBuilder sb, int index)
        {
            sb.AppendLine("Rectangle:" + index.ToString());
            sb.AppendLine("X1:" + X1.ToString());
            sb.AppendLine("X2:" + X2.ToString());
            sb.AppendLine("Y1:" + Y1.ToString());
            sb.AppendLine("Y2:" + Y2.ToString());
            return sb;
        }

        public void Load(List<LineData> data, int index)
        {
            X1 = int.Parse(data[index + 1].FieldValue);
            X2 = int.Parse(data[index + 2].FieldValue);
            Y1 = int.Parse(data[index + 3].FieldValue);
            Y2 = int.Parse(data[index + 4].FieldValue);
        }
    }
}

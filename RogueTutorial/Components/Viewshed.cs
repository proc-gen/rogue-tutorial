using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueTutorial.Interfaces;
using RogueTutorial.Utils;
using SadRogue.Primitives;
using SimpleECS;

namespace RogueTutorial.Components
{
    internal class Viewshed : ISaveableComponent
    {
        public List<Point> VisibleTiles { get; set; }
        public int Range { get; set; }
        public bool Dirty { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            Range = int.Parse(componentData[0].FieldValue);
            VisibleTiles = new List<Point>();
            Dirty = true;
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:Viewshed");
            sb.AppendLine("Range:" + Range.ToString());
            return sb;
        }
    }
}

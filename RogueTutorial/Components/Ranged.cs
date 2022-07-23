using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueTutorial.Interfaces;
using SimpleECS;
using RogueTutorial.Utils;

namespace RogueTutorial.Components
{
    internal class Ranged : ISaveableComponent
    {
        public int Range { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            Range = int.Parse(componentData[0].FieldValue);
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:Ranged");
            sb.AppendLine("Range:" + Range.ToString());
            return sb;
        }
    }
}

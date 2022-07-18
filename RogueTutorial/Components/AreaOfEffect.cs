using RogueTutorial.Interfaces;
using RogueTutorial.Utils;
using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Components
{
    internal class AreaOfEffect : ISaveableComponent
    {
        public int Radius { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            Radius = int.Parse(componentData[0].FieldValue);
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:AreaOfEffect");
            sb.AppendLine("Radius:" + Radius.ToString());
            return sb;
        }
    }
}

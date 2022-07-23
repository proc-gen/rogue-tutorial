using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueTutorial.Interfaces;
using RogueTutorial.Utils;
using SimpleECS;

namespace RogueTutorial.Components
{
    internal class Confusion : ISaveableComponent
    {
        public int Turns { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            Turns = int.Parse(componentData[0].FieldValue);
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:Confusion");
            sb.AppendLine("Turns:" + Turns.ToString());
            return sb;
        }
    }
}

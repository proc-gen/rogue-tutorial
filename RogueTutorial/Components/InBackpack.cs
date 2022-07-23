using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueTutorial.Interfaces;
using RogueTutorial.Utils;

namespace RogueTutorial.Components
{
    internal class InBackpack : ISaveableComponent
    {
        public Entity Owner { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            Owner = entities[int.Parse(componentData[0].FieldValue)];
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:InBackpack");
            sb.AppendLine("Owner:" + Array.IndexOf(entities, Owner).ToString());
            return sb;
        }
    }
}

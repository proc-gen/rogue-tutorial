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
    internal class DefenseBonus : ISaveableComponent
    {
        public int Defense { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            Defense = int.Parse(componentData[0].FieldValue);
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:DefenseBonus");
            sb.AppendLine("Power:" + Defense.ToString());
            return sb;
        }
    }
}

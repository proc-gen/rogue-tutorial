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
    internal class ProvidesHealing : ISaveableComponent
    {
        public int HealAmount { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            HealAmount = int.Parse(componentData[0].FieldValue);
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:ProvidesHealing");
            sb.AppendLine("HealAmount:" + HealAmount.ToString());
            return sb;
        }
    }
}

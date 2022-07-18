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
    internal class InflictsDamage : ISaveableComponent
    {
        public int Damage { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            Damage = int.Parse(componentData[0].FieldValue);
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:InflictsDamage");
            sb.AppendLine("Damage:" + Damage.ToString());
            return sb;
        }
    }
}

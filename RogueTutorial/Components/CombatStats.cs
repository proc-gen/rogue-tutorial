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
    internal class CombatStats : ISaveableComponent
    {
        public int MaxHp { get;set; }
        public int Hp { get;set; }
        public int Defense { get;set; }
        public int Power { get;set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            MaxHp = int.Parse(componentData[0].FieldValue);
            Hp = int.Parse(componentData[1].FieldValue);
            Defense = int.Parse(componentData[2].FieldValue);
            Power = int.Parse(componentData[3].FieldValue);
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:CombatStats");
            sb.AppendLine("MaxHp:" + MaxHp.ToString());
            sb.AppendLine("Hp:" + Hp.ToString());
            sb.AppendLine("Defense:" + Defense.ToString());
            sb.AppendLine("Power:" + Power.ToString());
            return sb;
        }
    }
}

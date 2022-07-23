﻿using RogueTutorial.Interfaces;
using RogueTutorial.Utils;
using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Components
{
    internal class MeleePowerBonus : ISaveableComponent
    {
        public int Power { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            Power = int.Parse(componentData[0].FieldValue);
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:MeleePowerBonus");
            sb.AppendLine("Power:" + Power.ToString());
            return sb;
        }
    }
}

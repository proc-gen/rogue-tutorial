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
    internal class ProvidesFood : ISaveableComponent
    {
        public void Load(List<LineData> componentData, Entity[] entities)
        {
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:ProvidesFood");
            return sb;
        }
    }
}

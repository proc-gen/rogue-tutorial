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
    internal class EntityMoved : ISaveableComponent
    {
        public void Load(List<LineData> componentData, Entity[] entities)
        {
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:EntityMoved");
            return sb;
        }
    }
}

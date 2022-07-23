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
    internal class Name : ISaveableComponent
    {
        public string EntityName { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            EntityName = componentData[0].FieldValue;
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:Name");
            sb.AppendLine("EntityName:" + EntityName.ToString());
            return sb;
        }
    }
}

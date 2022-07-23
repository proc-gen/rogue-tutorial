using RogueTutorial.Helpers;
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
    internal class HungerClock : ISaveableComponent
    {
        public HungerState State { get; set; }
        public int Duration { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            State = HungerStateExtensions.getHungerStateFromString(componentData[0].FieldValue);
            Duration = int.Parse(componentData[1].FieldValue);
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:HungerClock");
            sb.AppendLine("State:" + State.ToString());
            sb.AppendLine("Duration:" + Duration.ToString());
            return sb;
        }
    }
}

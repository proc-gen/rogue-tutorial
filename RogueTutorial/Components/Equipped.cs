using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueTutorial.Interfaces;
using RogueTutorial.Utils;
using RogueTutorial.Helpers;

namespace RogueTutorial.Components
{
    internal class Equipped : ISaveableComponent
    {
        public Entity Owner { get; set; }
        public EquipmentSlot Slot { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            Owner = entities[int.Parse(componentData[0].FieldValue)];
            Slot = EquipmentSlotExtensions.getEquipmentSlotFromString(componentData[1].FieldValue);
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:Equipped");
            sb.AppendLine("Owner:" + Array.IndexOf(entities, Owner).ToString());
            sb.AppendLine("Slot:" + Slot.ToString());
            return sb;
        }
    }
}

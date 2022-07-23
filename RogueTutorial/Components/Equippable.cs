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
    internal class Equippable : ISaveableComponent
    {
        public EquipmentSlot Slot { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            Slot = EquipmentSlotExtensions.getEquipmentSlotFromString(componentData[0].FieldValue);
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:Equippable");
            sb.AppendLine("Slot:" + Slot.ToString());
            return sb;
        }
    }
}

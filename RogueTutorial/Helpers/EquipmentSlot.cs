using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Helpers
{
    public enum EquipmentSlot
    {
        None,
        Melee,
        Shield
    }

    public static class EquipmentSlotExtensions 
    {
        public static EquipmentSlot getEquipmentSlotFromString(string slot)
        {
            switch (slot)
            {
                case "Melee":
                    return EquipmentSlot.Melee;
                    break;
                case "Shield":
                    return EquipmentSlot.Shield;
                    break;
                default:
                    return EquipmentSlot.None;
                    break;
            }
        }
    }
}

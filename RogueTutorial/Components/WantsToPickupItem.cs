using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Components
{
    internal class WantsToPickupItem
    {
        public Entity CollectedBy { get; set; }
        public Entity Item { get; set; }
    }
}

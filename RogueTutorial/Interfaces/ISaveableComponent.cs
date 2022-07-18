using RogueTutorial.Utils;
using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Interfaces
{
    internal interface ISaveableComponent
    {
        StringBuilder Save(StringBuilder sb, Entity[] entities);
        void Load(List<LineData> componentData, Entity[] entities);
    }
}

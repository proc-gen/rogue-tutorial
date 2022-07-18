using RogueTutorial.Utils;
using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Interfaces
{
    internal interface ISaveable
    {
        StringBuilder Save(StringBuilder sb, int index);
        void Load(List<LineData> data, int index);
    }
}

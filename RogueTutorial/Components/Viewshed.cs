using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadRogue.Primitives;

namespace RogueTutorial.Components
{
    internal class Viewshed
    {
        public List<Point> VisibleTiles { get; set; }
        public int Range { get; set; }
    }
}

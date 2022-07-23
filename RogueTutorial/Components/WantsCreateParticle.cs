using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadRogue.Primitives;
using SadConsole;

namespace RogueTutorial.Components
{
    internal class WantsCreateParticle
    {
        public Point Point { get; set; }
        public List<Point> AdditionalPoints { get; set; }
        public ColoredGlyph Glyph { get; set; }
        public float LifetimeMilliseconds { get; set; }
    }
}

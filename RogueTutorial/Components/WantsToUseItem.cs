﻿using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadRogue.Primitives;

namespace RogueTutorial.Components
{
    internal class WantsToUseItem
    {
        public Entity Item { get; set; }
        public Point? Target { get; set; }
    }
}

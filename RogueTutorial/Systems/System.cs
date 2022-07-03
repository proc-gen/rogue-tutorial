using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Systems
{
    public abstract class System
    {
        protected Query query;
        protected World world;
        public System(World world, Query query)
        {
            this.query = query;
            this.world = world;
        }

        public abstract void Run(TimeSpan delta);
    }
}

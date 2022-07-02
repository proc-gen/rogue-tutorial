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

        public System(Query query)
        {
            this.query = query;
        }

        public abstract void Run(TimeSpan delta);
    }
}

using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Components;

namespace RogueTutorial.Systems
{
    internal class ParticleRemovalSystem : ECSSystem
    {
        public ParticleRemovalSystem(World world) 
            : base(world, world.CreateQuery()
                                .Has<ParticleLifetime>())
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((Entity entity, ref ParticleLifetime lifetime) =>
            {
                lifetime.LifetimeMilliseconds -= (float)delta.TotalMilliseconds;
                if (lifetime.LifetimeMilliseconds < 0.0f)
                {
                    entity.Destroy();
                }
            });
        }
    }
}

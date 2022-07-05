using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Components;

namespace RogueTutorial.Systems
{
    internal class DamageSystem : ECSSystem
    {
        public DamageSystem(World world, Query query) : base(world, query)
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((Entity entity, ref CombatStats stats, ref SufferDamage damage) =>
            {
                stats.Hp -= damage.Amount.Sum();

                entity.Remove<SufferDamage>();
            });
        }
    }
}

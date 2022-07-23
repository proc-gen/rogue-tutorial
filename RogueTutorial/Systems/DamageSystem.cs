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
        public DamageSystem(World world) 
            : base(world, world.CreateQuery()
                                .Has<SufferDamage>())
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in Map.Map map, Entity entity, ref CombatStats stats, ref SufferDamage damage, ref Position position) =>
            {
                stats.Hp -= damage.Amount.Sum();
                map.SetBloodyCell(position.Point);
                entity.Remove<SufferDamage>();
            });
        }
    }
}

using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Components;

namespace RogueTutorial.Systems
{
    internal class DeleteTheDead : ECSSystem
    {
        public DeleteTheDead(World world, Query query) : base(world, query)
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in Map.Map map, Entity entity, ref CombatStats stats, ref Position position) =>
            {
                if(stats.Hp < 1)
                {
                    map.SetCellWalkable(position.Point.X, position.Point.Y, true);
                    
                    if (entity.Has<Player>())
                    {
                        System.Diagnostics.Debug.WriteLine("You are dead");
                    }
                    else
                    {
                        entity.Destroy();
                    }
                }
            });
        }
    }
}

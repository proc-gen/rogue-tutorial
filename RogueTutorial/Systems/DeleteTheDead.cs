using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Components;
using RogueTutorial.Helpers;

namespace RogueTutorial.Systems
{
    internal class DeleteTheDead : ECSSystem
    {
        public DeleteTheDead(World world, Query query) : base(world, query)
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in Map.Map map, in GameLog log, Entity entity, ref CombatStats stats, ref Position position, ref Name name) =>
            {
                if(stats.Hp < 1)
                {
                    map.SetCellWalkable(position.Point.X, position.Point.Y, true);
                    
                    if (entity.Has<Player>())
                    {
                        log.Entries.Add("You are dead");
                        world.SetData(RunState.PlayerDeath);
                    }
                    else
                    {
                        log.Entries.Add(name.EntityName + " is dead");
                        entity.Destroy();
                    }
                }
            });
        }
    }
}

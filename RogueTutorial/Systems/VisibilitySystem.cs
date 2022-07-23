using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Map;
using RogueTutorial.Components;
using RogueTutorial.Helpers;
using SadRogue.Primitives;

namespace RogueTutorial.Systems
{
    internal class VisibilitySystem : ECSSystem
    {
        public VisibilitySystem(World world) 
            : base(world, world.CreateQuery()
                                .Has<Position>()
                                .Has<Viewshed>())
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in Map.Map map, in Random random, in GameLog log, Entity entity, ref Viewshed visibility, ref Position position) => 
            {
                if (visibility.Dirty)
                {
                    visibility.VisibleTiles = map.ComputeFOV(position.Point.X, position.Point.Y, visibility.Range, true, entity.Has<Player>());
                    visibility.Dirty = false;
                }

                if (entity.Has<Player>())
                {
                    foreach (Point point in visibility.VisibleTiles)
                    {
                        List<Entity> entities = map.GetCellEntities(point);
                        if (entities.Any(a => a.Has<Hidden>()))
                        {
                            if (random.Next(24) == 0)
                            {
                                foreach (Entity hidden in entities)
                                {
                                    if (hidden.Has<Hidden>())
                                    {
                                        hidden.Remove<Hidden>();
                                        log.Entries.Add("You spotted a " + hidden.Get<Name>().EntityName + ".");
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}

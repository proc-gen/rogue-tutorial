using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Map;
using RogueTutorial.Components;
using RogueTutorial.Helpers;

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
            Entity player = PlayerFunctions.GetPlayer(world);

            query.Foreach((in Map.Map map, Entity entity, ref Viewshed visibility, ref Position position) => 
            {
                if (visibility.Dirty)
                {
                    visibility.VisibleTiles = map.ComputeFOV(position.Point.X, position.Point.Y, visibility.Range, true, entity.Has<Player>());
                    visibility.Dirty = false;
                }
            });
        }
    }
}

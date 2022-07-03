using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Map;
using RogueTutorial.Components;

namespace RogueTutorial.Systems
{
    internal class VisibilitySystem : System
    {
        private Query playerQuery;
        public VisibilitySystem(World world, Query query, Query playerQuery) : base(world, query)
        {
            this.playerQuery = playerQuery;
        }

        public override void Run(TimeSpan delta)
        {
            Map.Map map = world.GetData<Map.Map>();
            Entity player = playerQuery.GetEntities()[0];

            query.Foreach((Entity entity, ref Viewshed visibility, ref Position position) => { 
                visibility.VisibleTiles = map.ComputeFOV(position.Point.X, position.Point.Y, visibility.Range, true, entity.index == player.index && entity.version == player.version);
            });
        }
    }
}

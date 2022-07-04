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
    internal class MonsterSystem : ECSSystem
    {
        private Query playerQuery;
        public MonsterSystem(World world, Query query, Query playerQuery) : base(world, query)
        {
            this.playerQuery = playerQuery;
        }

        public override void Run(TimeSpan delta)
        {
            Entity player = playerQuery.GetEntities()[0];
            Position playerPosition = player.Get<Position>();
            query.Foreach((Entity entity, ref Position position, ref Viewshed visibility, ref Name name) =>
            {
                if (visibility.VisibleTiles.Contains(playerPosition.Point))
                {
                    System.Diagnostics.Debug.WriteLine(name.EntityName + " shouts insults!");
                }
            });
        }
    }
}

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
    internal class PositionSystem : ECSSystem
    {
        public PositionSystem(World world, Query query) : base(world, query)
        {
        }

        public override void Run(TimeSpan delta)
        {
            Map.Map map = world.GetData<Map.Map>();

            query.Foreach((Entity entity, ref Position position) =>
            {
                /*if (position.Dirty)
                {
                    map.SetCellWalkable(position.PreviousPoint.X, position.PreviousPoint.Y, true);
                    map.SetCellWalkable(position.Point.X, position.Point.Y, false);
                    position.Dirty = false;
                }*/
            });
        }
    }
}

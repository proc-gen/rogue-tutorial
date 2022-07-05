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
            map.ResetTileContent();

            query.Foreach((Entity entity, ref Position position, ref BlocksTile blocksTile) =>
            {
                map.AddCellEntity(entity, position.Point);
            });
        }
    }
}

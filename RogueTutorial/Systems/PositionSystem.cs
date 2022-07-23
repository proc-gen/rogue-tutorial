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
        public PositionSystem(World world) 
            : base(world, world.CreateQuery()
                                .Has<Position>())
        {
        }

        public override void Run(TimeSpan delta)
        {
            world.GetData<Map.Map>().ResetTileContent();

            query.Foreach((in Map.Map map, Entity entity, ref Position position) =>
            {
                map.AddCellEntity(entity, position.Point);
            });
        }
    }
}

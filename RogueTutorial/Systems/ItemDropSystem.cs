using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Components;
using RogueTutorial.Helpers;
using SadRogue.Primitives;

namespace RogueTutorial.Systems
{
    internal class ItemDropSystem : ECSSystem
    {
        public ItemDropSystem(World world) 
            : base(world, world.CreateQuery()
                                .Has<WantsToDropItem>())
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in GameLog log, Entity entity, ref WantsToDropItem dropItem, ref Name name, ref Position position) =>
            {
                dropItem.Item.Set(new Position() { Point = new Point(position.Point.X, position.Point.Y) });

                if (entity.Has<Player>())
                {
                    log.Entries.Add("You drop the " + dropItem.Item.Get<Name>().EntityName + ".");
                }

                dropItem.Item.Remove<InBackpack>();   
                entity.Remove<WantsToDropItem>();
            });
        }
    }
}

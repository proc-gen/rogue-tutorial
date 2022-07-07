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
    internal class ItemDropSystem : ECSSystem
    {
        public ItemDropSystem(World world, Query query) : base(world, query)
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in GameLog log, Entity entity, ref WantsToDropItem dropItem, ref Name name, ref Position position) =>
            {
                dropItem.Item.Set(new Position() { Point = position.Point });

                if (entity.Has<Player>())
                {
                    log.Entries.Add("You drop the " + dropItem.Item.Get<Name>().EntityName + ".");
                }

                dropItem.Item.Remove<InBackpack>();               
            });
        }
    }
}

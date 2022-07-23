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
    internal class ItemRemoveSystem : ECSSystem
    {
        public ItemRemoveSystem(World world, Query query) : base(world, query)
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in GameLog log, Entity entity, ref WantsToRemoveItem dropItem, ref Name name) =>
            {
                dropItem.Item.Set(new InBackpack() { Owner = entity });

                if (entity.Has<Player>())
                {
                    log.Entries.Add("You unequip the " + dropItem.Item.Get<Name>().EntityName + ".");
                }

                entity.Remove<WantsToRemoveItem>();
            });
        }
    }
}

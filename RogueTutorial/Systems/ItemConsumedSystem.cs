using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Components;

namespace RogueTutorial.Systems
{
    internal class ItemConsumedSystem : ECSSystem
    {
        public ItemConsumedSystem(World world, Query query) : base(world, query)
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((Entity entity, ref WantsToUseItem wantsUse) =>
            {
                if (wantsUse.Item.Has<Consumable>())
                {
                    Consumable consumable = wantsUse.Item.Get<Consumable>();
                    if (consumable.Consumed)
                    {
                        wantsUse.Item.Destroy();
                        entity.Remove<WantsToUseItem>();
                    }
                }
            });
        }
    }
}

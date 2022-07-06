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
    internal class ItemCollectionSystem : ECSSystem
    {
        public ItemCollectionSystem(World world, Query query) : base(world, query)
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in GameLog log, Entity entity, ref WantsToPickupItem wantsPickup) =>
            {
                wantsPickup.Item.Set(new InBackpack() { Owner = wantsPickup.CollectedBy });
                wantsPickup.Item.Remove<Position>();

                if (wantsPickup.CollectedBy.Has<Player>())
                {
                    log.Entries.Add("You pick up the " + wantsPickup.Item.Get<Name>().EntityName);
                }

                entity.Remove<WantsToPickupItem>();
            });
        }

        public static bool GetItem(World world, Entity collector)
        {
            bool needScreenRefresh = false;
            Position collectorPosition = collector.Get<Position>();
            IEnumerable<Entity> items = world.CreateQuery()
                                .Has<Item>().GetEntities()
                                .Where(a => a.IsValid() && a.Get<Position>().Point == collectorPosition.Point);

            if(items.Any())
            {
                collector.Set(new WantsToPickupItem() { CollectedBy = collector, Item = items.First()});
                needScreenRefresh = true;
            }
            else if(collector.Has<Player>())
            {
                world.GetData<GameLog>().Entries.Add("There is nothing here to pick up.");
                needScreenRefresh = true;
            }

            return needScreenRefresh;
        }
    }
}

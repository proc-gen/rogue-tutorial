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
        Query equippedItemsQuery;
        public ItemCollectionSystem(World world) 
            : base(world, world.CreateQuery()
                                .Has<WantsToPickupItem>())
        {
            equippedItemsQuery = world.CreateQuery().Has<Equipped>();
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in GameLog log, Entity entity, ref WantsToPickupItem wantsPickup) =>
            {
                bool pickedUp = false;
                if (wantsPickup.Item.Has<Equippable>())
                {
                    Equippable equippable = wantsPickup.Item.Get<Equippable>();
                    if(!equippedItemsQuery.GetEntities().Any(a => a.Get<Equipped>().Owner == entity && a.Get<Equipped>().Slot == equippable.Slot))
                    {
                        wantsPickup.Item.Set(new Equipped() { Owner = wantsPickup.CollectedBy, Slot = equippable.Slot });
                        wantsPickup.Item.Remove<Position>();

                        if (wantsPickup.CollectedBy.Has<Player>())
                        {
                            log.Entries.Add("You equip the " + wantsPickup.Item.Get<Name>().EntityName);
                        }
                        pickedUp = true;
                    }
                }

                if (!pickedUp)
                {
                    wantsPickup.Item.Set(new InBackpack() { Owner = wantsPickup.CollectedBy });
                    wantsPickup.Item.Remove<Position>();

                    if (wantsPickup.CollectedBy.Has<Player>())
                    {
                        log.Entries.Add("You pick up the " + wantsPickup.Item.Get<Name>().EntityName);
                    }
                }
                entity.Remove<WantsToPickupItem>();
            });
        }

        public static int GetItem(World world, Entity collector)
        {
            int status = 0;
            Position collectorPosition = collector.Get<Position>();
            IEnumerable<Entity> items = world.CreateQuery()
                                .Has<Item>().Has<Position>().GetEntities()
                                .Where(a => a.IsValid() && a.Get<Position>().Point == collectorPosition.Point);

            if(items.Any())
            {
                collector.Set(new WantsToPickupItem() { CollectedBy = collector, Item = items.First()});
                status = 1;
            }
            else if(collector.Has<Player>())
            {
                world.GetData<GameLog>().Entries.Add("There is nothing here to pick up.");
                status = 2;
            }

            return status;
        }
    }
}

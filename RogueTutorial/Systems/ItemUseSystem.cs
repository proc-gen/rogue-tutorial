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
    internal class ItemUseSystem : ECSSystem
    {
        Query equippedItemsQuery;
        public ItemUseSystem(World world, Query query) : base(world, query)
        {
            equippedItemsQuery = world.CreateQuery().Has<Equipped>();
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in GameLog log, in Map.Map map, Entity entity, ref WantsToUseItem wantsUse, ref Name name, ref CombatStats stats) =>
            {
                if (wantsUse.Item.Has<Equippable>())
                {
                    Equippable equippable = wantsUse.Item.Get<Equippable>();
                    if (equippedItemsQuery.GetEntities().Any(a => a.Get<Equipped>().Owner == entity && a.Get<Equipped>().Slot == equippable.Slot))
                    {
                        Entity equippedEntity = equippedItemsQuery.GetEntities().Where(a => a.Get<Equipped>().Owner == entity && a.Get<Equipped>().Slot == equippable.Slot).First();
                        equippedEntity.Set(new InBackpack() { Owner = entity });

                        if (entity.Has<Player>())
                        {
                            log.Entries.Add("You unequip the " + equippedEntity.Get<Name>().EntityName);
                        }
                    }
                    
                    wantsUse.Item.Set(new Equipped() { Owner = entity, Slot = equippable.Slot });
                    wantsUse.Item.Remove<Position>();

                    if (entity.Has<Player>())
                    {
                        log.Entries.Add("You equip the " + wantsUse.Item.Get<Name>().EntityName);
                    }
                }
                if (wantsUse.Item.Has<ProvidesHealing>())
                {
                    ProvidesHealing healer = wantsUse.Item.Get<ProvidesHealing>();
                    stats.Hp = Math.Min(stats.MaxHp, stats.Hp + healer.HealAmount);

                    if (entity.Has<Player>())
                    {
                        log.Entries.Add("You use the " + wantsUse.Item.Get<Name>().EntityName + ", healing " + healer.HealAmount + " hp.");
                    }

                    if (wantsUse.Item.Has<Consumable>())
                    {
                        wantsUse.Item.Get<Consumable>().Consumed = true;
                    }
                }
                else if (wantsUse.Item.Has<Ranged>())
                {
                    if (wantsUse.Target.HasValue)
                    {
                        List<Point> targetCells = new List<Point>();
                        if (wantsUse.Item.Has<AreaOfEffect>())
                        {
                            targetCells.AddRange(map.ComputeFOV(wantsUse.Target.Value.X, wantsUse.Target.Value.Y, wantsUse.Item.Get<AreaOfEffect>().Radius, false, false));
                        }
                        else
                        {
                            targetCells.Add(wantsUse.Target.Value);
                        }
                        IEnumerable<Entity> targets = world.CreateQuery().Has<Position>().Has<CombatStats>().GetEntities().Where(a => targetCells.Contains(a.Get<Position>().Point));

                        if (targets.Any())
                        {
                            string nameToUse = name.EntityName + " uses ";
                            if (entity.Has<Player>())
                            {
                                nameToUse = "You use ";
                            }

                            if (wantsUse.Item.Has<InflictsDamage>())
                            {
                                InflictsDamage inflicts = wantsUse.Item.Get<InflictsDamage>();
                                foreach (Entity target in targets)
                                {
                                    SufferDamage targetDamage = null;
                                    if (!target.TryGet(out targetDamage))
                                    {
                                        targetDamage = new SufferDamage();
                                    }
                                    targetDamage.NewDamage(inflicts.Damage);
                                    target.Set(targetDamage);

                                    log.Entries.Add(nameToUse + wantsUse.Item.Get<Name>().EntityName + " on " + target.Get<Name>().EntityName + ", inflicting " + inflicts.Damage + " hp");
                                }
                            }

                            if (wantsUse.Item.Has<Confusion>())
                            {
                                Confusion confusion = wantsUse.Item.Get<Confusion>();
                                foreach(Entity target in targets)
                                {
                                    Confusion targetConfusion = null;
                                    if (!target.TryGet(out targetConfusion))
                                    {
                                        targetConfusion = new Confusion();
                                    }
                                    targetConfusion.Turns = confusion.Turns;
                                    target.Set(targetConfusion);

                                    log.Entries.Add(nameToUse + wantsUse.Item.Get<Name>().EntityName + " on " + target.Get<Name>().EntityName + ", confusing them");
                                }
                            }
                        }

                        if (wantsUse.Item.Has<Consumable>())
                        {
                            wantsUse.Item.Get<Consumable>().Consumed = true;
                        }
                    }
                    else
                    {
                        Ranged ranged = wantsUse.Item.Get<Ranged>();
                        entity.Set(new UseForTargeting() { Item = wantsUse.Item });
                        world.SetData(RunState.ShowTargeting);
                    }
                }
            });
        }
    }
}

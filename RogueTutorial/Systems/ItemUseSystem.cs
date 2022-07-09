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
        public ItemUseSystem(World world, Query query) : base(world, query)
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in GameLog log, Entity entity, ref WantsToUseItem wantsUse, ref Name name, ref CombatStats stats) =>
            {
                if (wantsUse.Item.Has<ProvidesHealing>())
                {
                    ProvidesHealing healer = wantsUse.Item.Get<ProvidesHealing>();
                    stats.Hp = Math.Min(stats.MaxHp, stats.Hp + healer.HealAmount);

                    if (entity.Has<Player>())
                    {
                        log.Entries.Add("You drink the " + wantsUse.Item.Get<Name>().EntityName + ", healing " + healer.HealAmount + " hp.");
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
                        Point targetCell = wantsUse.Target.Value;
                        IEnumerable<Entity> targets = world.CreateQuery().Has<Position>().Has<CombatStats>().GetEntities().Where(a => a.Get<Position>().Point == targetCell);

                        if (targets.Any())
                        {
                            string nameToUse = name.EntityName + " uses ";
                            if (entity.Has<Player>())
                            {
                                nameToUse = "You use ";
                            }

                            InflictsDamage inflicts = wantsUse.Item.Get<InflictsDamage>();
                            foreach(Entity target in targets)
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

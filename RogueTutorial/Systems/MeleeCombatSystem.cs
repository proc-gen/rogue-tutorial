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
    internal class MeleeCombatSystem : ECSSystem
    {
        Query equippedMeleeAttackItems,
                equippedDefenseItems;
        public MeleeCombatSystem(World world, Query query) : base(world, query)
        {
            equippedMeleeAttackItems = world.CreateQuery().Has<Equipped>().Has<MeleePowerBonus>();
            equippedDefenseItems = world.CreateQuery().Has<Equipped>().Has<DefenseBonus>();
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in GameLog log, Entity entity, ref WantsToMelee wantsMelee, ref Name name, ref CombatStats stats) =>
            {
                if(stats.Hp > 0)
                {
                    CombatStats targetStats = wantsMelee.Target.Get<CombatStats>();
                    if(targetStats.Hp > 0)
                    {
                        Name targetName = wantsMelee.Target.Get<Name>();
                        int damage = calculateTargetDamage(entity, wantsMelee, stats, targetStats);

                        if(damage > 0)
                        {
                            SufferDamage targetDamage = null;
                            if (!wantsMelee.Target.TryGet(out targetDamage))
                            {
                                targetDamage = new SufferDamage();
                            }
                            targetDamage.NewDamage(damage);
                            wantsMelee.Target.Set(targetDamage);

                            log.Entries.Add(name.EntityName + " hits " + targetName.EntityName + ", for " + damage + " hp");
                        }
                        else
                        {
                            log.Entries.Add(name.EntityName + " is unable to hurt " + targetName.EntityName);
                        }
                    }
                }

                entity.Remove<WantsToMelee>();
            });
        }

        private int calculateTargetDamage(Entity attacker, WantsToMelee wantsMelee, CombatStats stats, CombatStats targetStats)
        {
            int damage = Math.Max(0, getAttackerPower(attacker, stats) - getTargetDefense(wantsMelee.Target, targetStats));

            return damage;
        }

        private int getAttackerPower(Entity attacker, CombatStats stats)
        {
            int power = stats.Power;

            IEnumerable<Entity> meleeAttackItems = equippedMeleeAttackItems.GetEntities().Where(a => a.Get<Equipped>().Owner == attacker);
            foreach (Entity item in meleeAttackItems)
            {
                power += item.Get<MeleePowerBonus>().Power;
            }

            return power;
        }

        private int getTargetDefense(Entity target, CombatStats stats)
        {
            int defense = stats.Defense;

            IEnumerable<Entity> defenseItems = equippedDefenseItems.GetEntities().Where(a => a.Get<Equipped>().Owner == target);
            foreach (Entity item in defenseItems)
            {
                defense += item.Get<DefenseBonus>().Defense;
            }

            return defense;
        }
    }
}

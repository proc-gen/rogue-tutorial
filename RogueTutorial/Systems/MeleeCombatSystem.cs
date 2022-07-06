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
        public MeleeCombatSystem(World world, Query query) : base(world, query)
        {
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
                        int damage = Math.Max(0, stats.Power - targetStats.Defense);

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
    }
}

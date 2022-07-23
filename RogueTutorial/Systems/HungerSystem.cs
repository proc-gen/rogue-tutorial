using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Components;
using SadConsole;
using RogueTutorial.Helpers;

namespace RogueTutorial.Systems
{
    internal class HungerSystem : ECSSystem
    {
        public HungerSystem(World world) 
            : base(world, world.CreateQuery()
                                .Has<HungerClock>())
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in GameLog log, in RunState runState, Entity entity, ref HungerClock hungerClock) =>
            {
                bool proceed = (runState == RunState.PlayerTurn && entity.Has<Player>()) 
                                || (runState == RunState.MonsterTurn && entity.Has<Monster>());

                if (proceed)
                {
                    hungerClock.Duration--;
                    if(hungerClock.Duration < 1)
                    {
                        if (entity.Has<Player>())
                        {
                            switch (hungerClock.State)
                            {
                                case HungerState.WellFed:
                                    log.Entries.Add("You are no longer well fed.");
                                    hungerClock.State = HungerState.Normal;
                                    break;
                                case HungerState.Normal:
                                    log.Entries.Add("You are hungry.");
                                    hungerClock.State = HungerState.Hungry;
                                    break;
                                case HungerState.Hungry:
                                    log.Entries.Add("You are starving!");
                                    hungerClock.State = HungerState.Starving;
                                    break;
                                case HungerState.Starving:
                                    log.Entries.Add("Your hunger pangs are getting painful!");
                                    break;
                            }
                        }

                        if (hungerClock.State > HungerState.Starving)
                        {
                            hungerClock.Duration = 200;
                        }
                        else
                        {
                            SufferDamage targetDamage = null;
                            if (!entity.TryGet(out targetDamage))
                            {
                                targetDamage = new SufferDamage();
                            }
                            targetDamage.NewDamage(1);
                            entity.Set(targetDamage);
                        }
                    }
                }
            });
        }
    }
}

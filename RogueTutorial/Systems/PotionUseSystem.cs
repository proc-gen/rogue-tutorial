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
    internal class PotionUseSystem : ECSSystem
    {
        public PotionUseSystem(World world, Query query) : base(world, query)
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in GameLog log, Entity entity, ref WantsToDrinkPotion wantsDrink, ref Name name, ref CombatStats stats) =>
            {
                Potion potion = wantsDrink.Potion.Get<Potion>();
                stats.Hp = Math.Min(stats.MaxHp, stats.Hp + potion.HealAmount);

                if (entity.Has<Player>())
                {
                    log.Entries.Add("You drink the " + wantsDrink.Potion.Get<Name>().EntityName + ", healing " + potion.HealAmount + " hp.");
                }

                wantsDrink.Potion.Destroy();
                entity.Remove<WantsToDrinkPotion>();
                
            });
        }
    }
}

using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Components;
using RogueTutorial.Helpers;
using SadRogue.Primitives;
using SadConsole;

namespace RogueTutorial.Systems
{
    internal class TriggerSystem : ECSSystem
    {
        public TriggerSystem(World world) 
            : base(world, world.CreateQuery()
                                .Has<EntityMoved>())
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((in GameLog log, in Map.Map map, Entity entity, ref Position position, ref EntityMoved entityMoved, ref Name name) =>
            {
                List<Entity> cellEntities = map.GetCellEntities(position.Point);
                if(cellEntities.Any(a => a != entity && a.Has<EntryTrigger>()))
                {
                    Entity trigger = cellEntities.Where(a => a != entity && a.Has<EntryTrigger>()).First();
                    log.Entries.Add(name.EntityName + " triggers " + trigger.Get<Name>().EntityName + "!");
                    if (trigger.Has<Hidden>())
                    {
                        trigger.Remove<Hidden>();
                    }
                    if (trigger.Has<InflictsDamage>())
                    {
                        entity.Set(new WantsCreateParticle() { LifetimeMilliseconds = 200.0f, Point = position.Point, Glyph = new ColoredGlyph(Color.Orange, Color.Black, 19) });
                        InflictsDamage inflicts = trigger.Get<InflictsDamage>();
                        SufferDamage targetDamage = null;
                        if (!entity.TryGet(out targetDamage))
                        {
                            targetDamage = new SufferDamage();
                        }
                        targetDamage.NewDamage(inflicts.Damage);
                        entity.Set(targetDamage);
                    }
                    if (trigger.Has<SingleActivation>())
                    {
                        trigger.Destroy();
                    }
                }


                entity.Remove<EntityMoved>();
            });
        }
    }
}

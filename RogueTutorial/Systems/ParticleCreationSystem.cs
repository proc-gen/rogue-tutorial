using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Components;
using SadConsole;

namespace RogueTutorial.Systems
{
    internal class ParticleCreationSystem : ECSSystem
    {
        public ParticleCreationSystem(World world) 
            : base(world, world.CreateQuery()
                                .Has<WantsCreateParticle>())
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((Entity entity, ref WantsCreateParticle wantsParticle) =>
            {
                world.CreateEntity(new Position() { Point = wantsParticle.Point }
                                    , new ParticleLifetime() { LifetimeMilliseconds = wantsParticle.LifetimeMilliseconds}
                                    , new Renderable() { Glyph = new ColoredGlyph(wantsParticle.Glyph.Foreground, wantsParticle.Glyph.Background, wantsParticle.Glyph.Glyph), RenderOrder = 99});
                if(wantsParticle.AdditionalPoints != null)
                {
                    foreach(var point in wantsParticle.AdditionalPoints)
                    {
                        world.CreateEntity(new Position() { Point = point }
                                    , new ParticleLifetime() { LifetimeMilliseconds = wantsParticle.LifetimeMilliseconds }
                                    , new Renderable() { Glyph = new ColoredGlyph(wantsParticle.Glyph.Foreground, wantsParticle.Glyph.Background, wantsParticle.Glyph.Glyph), RenderOrder = 99 });
                    }
                }
                entity.Remove<WantsCreateParticle>();
            });
        }
    }
}

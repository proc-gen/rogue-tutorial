using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Map;
using RogueTutorial.Components;
using RogueTutorial.Helpers;

using RogueSharp;

using Point = SadRogue.Primitives.Point;
using SadConsole;
using SadRogue.Primitives;

namespace RogueTutorial.Systems
{
    internal class MonsterSystem : ECSSystem
    {
        public MonsterSystem(World world) 
            : base(world, world.CreateQuery()
                                .Has<Position>()
                                .Has<Viewshed>()
                                .Has<Monster>())
        {
        }

        public override void Run(TimeSpan delta)
        {
            Entity player = PlayerFunctions.GetPlayer(world);
            Position playerPosition = player.Get<Position>();
            RunState runState = world.GetData<RunState>();

            if (runState == RunState.MonsterTurn)
            {
                query.Foreach((in Map.Map map, Entity entity, ref Position position, ref Viewshed visibility, ref Name name) =>
                {
                    bool canAct = true;
                    if (entity.Has<Confusion>())
                    {
                        canAct = false;
                        Confusion confusion = entity.Get<Confusion>();
                        entity.Set(new WantsCreateParticle() { LifetimeMilliseconds = 200.0f, Point = position.Point, Glyph = new ColoredGlyph(Color.Magenta, Color.Black, '?') });
                        if (confusion.Turns == 1)
                        {
                            entity.Remove<Confusion>();
                        }
                        else
                        {
                            confusion.Turns--;
                            entity.Set(confusion);
                        }
                    }
                    if (canAct)
                    {
                        if (visibility.VisibleTiles.Contains(playerPosition.Point))
                        {
                            if (Point.EuclideanDistanceMagnitude(playerPosition.Point, position.Point) <= 2.0)
                            {
                                entity.Set(new WantsToMelee() { Target = player });
                            }
                            else
                            {
                                Path path = map.FindPath(position.Point, playerPosition.Point);
                                if (path != null && path.Steps.ElementAtOrDefault(1) != null)
                                {
                                    if (path.Steps.Count() > 2)
                                    {
                                        ICell nextCell = path.Steps.ElementAt(1);
                                        if (map.IsCellWalkable(nextCell.X, nextCell.Y))
                                        {
                                            position.Point = new Point(nextCell.X, nextCell.Y);
                                            visibility.Dirty = true;
                                            map.SetCellWalkable(position.PreviousPoint.X, position.PreviousPoint.Y, true);
                                            map.SetCellWalkable(position.Point.X, position.Point.Y, false);
                                            position.Dirty = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }
    }
}

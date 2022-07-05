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

namespace RogueTutorial.Systems
{
    internal class MonsterSystem : ECSSystem
    {
        private Query playerQuery;
        public MonsterSystem(World world, Query query, Query playerQuery) : base(world, query)
        {
            this.playerQuery = playerQuery;
        }

        public override void Run(TimeSpan delta)
        {
            Entity player = playerQuery.GetEntities()[0];
            Position playerPosition = player.Get<Position>();
            RunState runState = world.GetData<RunState>();

            if (runState == RunState.MonsterTurn)
            {
                query.Foreach((in Map.Map map, Entity entity, ref Position position, ref Viewshed visibility, ref Name name) =>
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
                });
            }
        }
    }
}

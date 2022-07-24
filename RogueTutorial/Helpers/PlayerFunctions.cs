using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleECS;
using RogueTutorial.Components;
using SadRogue.Primitives;
using RogueTutorial.Map;

namespace RogueTutorial.Helpers
{
    public static class PlayerFunctions
    {
        public static Entity GetPlayer(World world)
        {
            return world.CreateQuery().Has<Player>().GetEntities().First();
        }

        public static bool PlayerExists(World world)
        {
            return world.CreateQuery().Has<Player>().GetEntities().Any();
        }

        public static bool TryMovePlayer(World world, Direction direction)
        {
            bool retVal = false;
            Map.Map map = world.GetData<Map.Map>();
            Entity player = GetPlayer(world);

            Position position = player.Get<Position>();
            Viewshed visibility = player.Get<Viewshed>();
            Name name = player.Get<Name>();

            Point newPoint = position.Point.Add(direction);
            if (map.IsCellWalkable(newPoint.X, newPoint.Y))
            {
                position.Point = newPoint;
                visibility.Dirty = true;

                map.SetCellWalkable(position.PreviousPoint.X, position.PreviousPoint.Y, true);
                map.SetCellWalkable(position.Point.X, position.Point.Y, false);
                position.Dirty = false;
                player.Set(new EntityMoved());
                retVal = true;
            }
            else
            {
                List<Entity> cellEntities = map.GetCellEntities(newPoint);
                if (cellEntities.Count > 0)
                {
                    Entity monster = cellEntities.Where(a => a.Has<Monster>()).FirstOrDefault();
                    if (monster)
                    {
                        player.Set(new WantsToMelee() { Target = monster });
                        retVal = true;
                    }
                }
            }

            return retVal;
        }

        public static bool TryPlayerDescend(World world)
        {
            bool retVal = false;

            Map.Map map = world.GetData<Map.Map>();
            Entity player = GetPlayer(world);

            Position position = player.Get<Position>();
            if (map.GetMapCell(position.Point.X, position.Point.Y) == TileType.DownStairs)
            {
                retVal = true;
            }
            else
            {
                world.GetData<GameLog>().Entries.Add("There is no way down from here.");
            }
            return retVal;
        }

        public static void SkipPlayerTurn(World world)
        {
            Map.Map map = world.GetData<Map.Map>();
            Entity player = GetPlayer(world);

            Position position = player.Get<Position>();
            Viewshed visibility = player.Get<Viewshed>();
            CombatStats combatStats = player.Get<CombatStats>();
            HungerClock hungerClock = player.Get<HungerClock>();

            bool canHeal = hungerClock.State > HungerState.Hungry;

            if (canHeal)
            {
                foreach (Point visible in visibility.VisibleTiles)
                {
                    if (map.GetCellEntities(visible).Any(a => a.Has<Monster>()))
                    {
                        canHeal = false;
                    }
                }
            }

            if (canHeal)
            {
                combatStats.Hp = Math.Min(combatStats.Hp + 1, combatStats.MaxHp);
            }
        }
    }
}

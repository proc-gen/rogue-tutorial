using RogueTutorial.Components;
using SadRogue.Primitives;
using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;

namespace RogueTutorial.Map
{
    internal class Spawner
    {
        const int MAX_MONSTERS = 4;
        const int MAX_ITEMS = 2;

        public static Entity SpawnPlayer(World world, Point playerStart)
        {
            return world.CreateEntity(new Position() { Point =  playerStart}
                                , new Renderable() { Glyph = new ColoredGlyph(Color.Yellow, Color.Black, '@') }
                                , new Player()
                                , new Viewshed() { VisibleTiles = new List<Point>(), Range = 8, Dirty = true }
                                , new Name() { EntityName = "Player" }
                                , new BlocksTile()
                                , new CombatStats() { MaxHp = 30, Hp = 30, Defense = 2, Power = 5 });
        }

        public static Entity SpawnMonster(World world, Point monsterStart)
        {
            Random random = world.GetData<Random>();

            switch (random.Next(2))
            {
                case 0:
                    return spawnGoblin(world, monsterStart);
                    break;
                default:
                    return spawnOrc(world, monsterStart);
                    break;
            }
        }

        private static Entity spawnOrc(World world, Point monsterStart)
        {
            return spawnMonster(world, monsterStart, "Orc", 'o');
        }

        private static Entity spawnGoblin(World world, Point monsterStart)
        {
            return spawnMonster(world, monsterStart, "Goblin", 'g');
        }

        private static Entity spawnMonster(World world, Point monsterStart, string name, char tile)
        {
            return world.CreateEntity(new Position() { Point = monsterStart }
                                        , new Renderable() { Glyph = new ColoredGlyph(Color.Red, Color.Black, tile) }
                                        , new Viewshed() { VisibleTiles = new List<Point>(), Range = 8, Dirty = true }
                                        , new Monster()
                                        , new Name() { EntityName = name }
                                        , new BlocksTile()
                                        , new CombatStats() { MaxHp = 16, Hp = 16, Defense = 1, Power = 4 });
        }
    
        public static void SpawnRoom(World world, Rectangle room)
        {
            Random random = world.GetData<Random>();

            List<Point> spawnPoints = new List<Point>();
            int numMonsters = random.Next(MAX_MONSTERS + 1);

            for(int i = 0; i < numMonsters; i++)
            {
                bool added = false;
                Point spawnPoint = Point.None;
                while (!added)
                {
                    spawnPoint = new Point(room.X1 + random.Next(1, Math.Abs(room.X2 - room.X1))
                                            , room.Y1 + random.Next(1, room.Y2 - room.Y1));
                    if (!spawnPoints.Contains(spawnPoint))
                    {
                        added = true;
                    }
                };
                spawnPoints.Add(spawnPoint);
            }

            foreach(Point spawnPoint in spawnPoints)
            {
                SpawnMonster(world, spawnPoint);
            }
        }
    
    }
}

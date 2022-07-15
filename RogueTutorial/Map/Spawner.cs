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
                                , new Renderable() { Glyph = new ColoredGlyph(Color.Yellow, Color.Black, '@'), RenderOrder = 0 }
                                , new Player()
                                , new Viewshed() { VisibleTiles = new List<Point>(), Range = 8, Dirty = true }
                                , new Name() { EntityName = "Player" }
                                , new BlocksTile()
                                , new CombatStats() { MaxHp = 30, Hp = 30, Defense = 2, Power = 5 });
        }

        public static void SpawnRoom(World world, Rectangle room)
        {
            spawnMonsters(world, room);
            spawnItems(world, room);
        }

        private static void spawnMonsters(World world, Rectangle room)
        {
            Random random = world.GetData<Random>();

            List<Point> spawnPoints = new List<Point>();
            int numMonsters = random.Next(MAX_MONSTERS + 1);

            for (int i = 0; i < numMonsters; i++)
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

            foreach (Point spawnPoint in spawnPoints)
            {
                SpawnMonster(world, spawnPoint);
            }
        }

        private static void spawnItems(World world, Rectangle room)
        {
            Random random = world.GetData<Random>();

            List<Point> spawnPoints = new List<Point>();
            int numItems = random.Next(MAX_ITEMS + 1);

            for (int i = 0; i < numItems; i++)
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

            foreach (Point spawnPoint in spawnPoints)
            {
                SpawnItem(world, spawnPoint);
            }
        }

        public static Entity SpawnMonster(World world, Point start)
        {
            Random random = world.GetData<Random>();

            switch (random.Next(2))
            {
                case 0:
                    return spawnGoblin(world, start);
                    break;
                default:
                    return spawnOrc(world, start);
                    break;
            }
        }

        private static Entity spawnOrc(World world, Point start)
        {
            return spawnMonster(world, start, "Orc", 'o');
        }

        private static Entity spawnGoblin(World world, Point start)
        {
            return spawnMonster(world, start, "Goblin", 'g');
        }

        private static Entity spawnMonster(World world, Point start, string name, char tile)
        {
            return world.CreateEntity(new Position() { Point = start }
                                        , new Renderable() { Glyph = new ColoredGlyph(Color.Red, Color.Black, tile), RenderOrder = 1 }
                                        , new Viewshed() { VisibleTiles = new List<Point>(), Range = 8, Dirty = true }
                                        , new Monster()
                                        , new Name() { EntityName = name }
                                        , new BlocksTile()
                                        , new CombatStats() { MaxHp = 16, Hp = 16, Defense = 1, Power = 4 });
        }

        public static Entity SpawnItem(World world, Point start)
        {
            Random random = world.GetData<Random>();

            switch (random.Next(4))
            {
                case 0:
                    return spawnHealthPotion(world, start);
                    break;
                case 1:
                    return spawnMagicMissileScroll(world, start);
                    break;
                case 2:
                    return spawnConfusionScroll(world, start);
                    break;
                default:
                    return spawnFireballScroll(world, start);
            }
        }

        private static Entity spawnHealthPotion(World world, Point start)
        {
            return world.CreateEntity(new Position() { Point = start }
                                        , new Renderable() { Glyph = new ColoredGlyph(Color.Magenta, Color.Black, 173), RenderOrder = 2 }
                                        , new Name() { EntityName = "Health Potion" }
                                        , new Item()
                                        , new Consumable()
                                        , new ProvidesHealing() { HealAmount = 8 });
                        
        }

        private static Entity spawnMagicMissileScroll(World world, Point start)
        {
            return world.CreateEntity(new Position() { Point = start }
                                        , new Renderable() { Glyph = new ColoredGlyph(Color.Cyan, Color.Black, ')'), RenderOrder = 2 }
                                        , new Name() { EntityName = "Magic Missile Scroll" }
                                        , new Item()
                                        , new Consumable()
                                        , new Ranged() { Range = 6 }
                                        , new InflictsDamage() { Damage = 8});
        }

        private static Entity spawnFireballScroll(World world, Point start)
        {
            return world.CreateEntity(new Position() { Point = start }
                                        , new Renderable() { Glyph = new ColoredGlyph(Color.Orange, Color.Black, ')'), RenderOrder = 2 }
                                        , new Name() { EntityName = "Fireball Scroll" }
                                        , new Item()
                                        , new Consumable()
                                        , new Ranged() { Range = 6 }
                                        , new InflictsDamage() { Damage = 20 }
                                        , new AreaOfEffect() { Radius = 3 });
        }

        private static Entity spawnConfusionScroll(World world, Point start)
        {
            return world.CreateEntity(new Position() { Point = start }
                                        , new Renderable() { Glyph = new ColoredGlyph(Color.Pink, Color.Black, ')'), RenderOrder = 2 }
                                        , new Name() { EntityName = "Confusion Scroll" }
                                        , new Item()
                                        , new Consumable()
                                        , new Ranged() { Range = 6 }
                                        , new Confusion() { Turns = 4 });
        }

    }
}

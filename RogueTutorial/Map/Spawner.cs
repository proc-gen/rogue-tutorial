using RogueTutorial.Components;
using SadRogue.Primitives;
using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using RogueTutorial.Utils;
using RogueTutorial.Helpers;

namespace RogueTutorial.Map
{
    internal class Spawner
    {
        const int MAX_MONSTERS = 4;

        public static Entity SpawnPlayer(World world, Point playerStart)
        {
            return world.CreateEntity(new Position() { Point =  playerStart}
                                , new Renderable() { Glyph = new ColoredGlyph(Color.Yellow, Color.Black, '@'), RenderOrder = 0 }
                                , new Player()
                                , new Viewshed() { VisibleTiles = new List<Point>(), Range = 8, Dirty = true }
                                , new Name() { EntityName = "Player" }
                                , new BlocksTile()
                                , new CombatStats() { MaxHp = 30, Hp = 30, Defense = 2, Power = 5 }
                                , new HungerClock() { State = HungerState.WellFed, Duration = 20});
        }

        public static void SpawnRoom(World world, Rectangle room)
        {
            int depth = world.GetData<Map>().Depth;
            List<Point> spawnPoints = getNonPlayerSpawnPoints(world, room, depth);
            spawnNonPlayerEntities(world, spawnPoints, depth);
        }

        private static RandomTable roomTable(int depth)
        {
            return new RandomTable()
                        .Add("Goblin", 10)
                        .Add("Orc", 1 + depth)
                        .Add("Health Potion", 7)
                        .Add("Fireball Scroll", 2 + depth)
                        .Add("Confusion Scroll", 2 + depth)
                        .Add("Magic Missile Scroll", 4)
                        .Add("Dagger", 3)
                        .Add("Shield", 3)
                        .Add("Long Sword", depth - 1)
                        .Add("Tower Shield", depth - 1)
                        .Add("Rations", 10);
        }

        private static List<Point> getNonPlayerSpawnPoints(World world, Rectangle room, int depth)
        {
            Random random = world.GetData<Random>();

            List<Point> spawnPoints = new List<Point>();
            int numPoints = random.Next(1, MAX_MONSTERS + 4) + (depth - 1) - 3;

            for (int i = 0; i < numPoints; i++)
            {
                bool added = false;
                Point spawnPoint = Point.None;
                int tries = 0;
                while (!added && tries < 20)
                {
                    spawnPoint = new Point(room.X1 + random.Next(1, Math.Abs(room.X2 - room.X1))
                                            , room.Y1 + random.Next(1, room.Y2 - room.Y1));
                    if (!spawnPoints.Contains(spawnPoint))
                    {
                        added = true;
                    }
                    else
                    {
                        tries++;
                    }
                };
                spawnPoints.Add(spawnPoint);
            }

            return spawnPoints;            
        }


        private static void spawnNonPlayerEntities(World world, List<Point> spawnPoints, int depth)
        {
            Random random = world.GetData<Random>();
            RandomTable randomTable = roomTable(depth);

            foreach (Point spawnPoint in spawnPoints)
            {
                switch (randomTable.Roll(random))
                {
                    case "Goblin":
                        spawnGoblin(world, spawnPoint);
                        break;
                    case "Orc":
                        spawnOrc(world, spawnPoint);
                        break;
                    case "Health Potion":
                        spawnHealthPotion(world, spawnPoint);
                        break;
                    case "Fireball Scroll":
                        spawnFireballScroll(world, spawnPoint);
                        break;
                    case "Confusion Scroll":
                        spawnConfusionScroll(world, spawnPoint);
                        break;
                    case "Magic Missile Scroll":
                        spawnMagicMissileScroll(world, spawnPoint);
                        break;
                    case "Dagger":
                        spawnDagger(world, spawnPoint);
                        break;
                    case "Shield":
                        spawnShield(world, spawnPoint);
                        break;
                    case "Long Sword":
                        spawnLongSword(world, spawnPoint);
                        break;
                    case "Tower Shield":
                        spawnTowerShield(world, spawnPoint);
                        break;
                    case "Rations":
                        spawnRations(world, spawnPoint);
                        break;
                }
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

        private static Entity spawnDagger(World world, Point start)
        {
            return world.CreateEntity(new Position() { Point = start }
                                        , new Renderable() { Glyph = new ColoredGlyph(Color.Cyan, Color.Black, '/'), RenderOrder = 2 }
                                        , new Name() { EntityName = "Dagger" }
                                        , new Item()
                                        , new Equippable() { Slot = EquipmentSlot.Melee }
                                        , new MeleePowerBonus() { Power = 2 });
        }

        private static Entity spawnLongSword(World world, Point start)
        {
            return world.CreateEntity(new Position() { Point = start }
                                        , new Renderable() { Glyph = new ColoredGlyph(Color.Yellow, Color.Black, '/'), RenderOrder = 2 }
                                        , new Name() { EntityName = "Long Sword" }
                                        , new Item()
                                        , new Equippable() { Slot = EquipmentSlot.Melee }
                                        , new MeleePowerBonus() { Power = 4 });
        }

        private static Entity spawnShield(World world, Point start)
        {
            return world.CreateEntity(new Position() { Point = start }
                                        , new Renderable() { Glyph = new ColoredGlyph(Color.Cyan, Color.Black, '('), RenderOrder = 2 }
                                        , new Name() { EntityName = "Shield" }
                                        , new Item()
                                        , new Equippable() { Slot = EquipmentSlot.Shield }
                                        , new DefenseBonus() { Defense = 1 });
        }

        private static Entity spawnTowerShield(World world, Point start)
        {
            return world.CreateEntity(new Position() { Point = start }
                                        , new Renderable() { Glyph = new ColoredGlyph(Color.Yellow, Color.Black, '('), RenderOrder = 2 }
                                        , new Name() { EntityName = "Tower Shield" }
                                        , new Item()
                                        , new Equippable() { Slot = EquipmentSlot.Shield }
                                        , new DefenseBonus() { Defense = 3 });
        }

        private static Entity spawnRations(World world, Point start)
        {
            return world.CreateEntity(new Position() { Point = start }
                                        , new Renderable() { Glyph = new ColoredGlyph(Color.Green, Color.Black, '%'), RenderOrder = 2 }
                                        , new Name() { EntityName = "Rations" }
                                        , new Item()
                                        , new ProvidesFood()
                                        , new Consumable());
        }
    }
}

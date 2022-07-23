using RogueTutorial.Components;
using RogueTutorial.Interfaces;
using SadRogue.Primitives;
using SimpleECS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Utils
{
    public static class SaveGameManager
    {
        public static void SaveGame(World world)
        {
            StringBuilder sb = new StringBuilder();
            Entity[] entities = world.GetEntities();

            sb.AppendLine("EntityCount:" + entities.Length);
            for (int i = 0; i < entities.Length; i++)
            {
                sb.AppendLine("Entity:" + i.ToString());
                object[] components = entities[i].GetAllComponents().Where(a => a is ISaveableComponent).ToArray();
                if (components.Any())
                {
                    for (int j = 0; j < components.Length; j++)
                    {
                        if (components[j] is ISaveableComponent)
                        {
                            sb = ((ISaveableComponent)components[j]).Save(sb, entities);
                        }
                    }
                }
            }

            sb = world.GetData<Map.Map>().Save(sb, 0);

            deleteSaveData();

            using (StreamWriter file = new StreamWriter("savegame.txt"))
            {
                file.Write(sb.ToString());
            }
        }

        private static void deleteSaveData()
        {
            if (File.Exists("savegame.txt"))
            {
                File.Delete("savegame.txt");
            }
        }

        public static World LoadGame(World world)
        {
            world.SetData(new Random());

            List<LineData> fileData = getFileData();
            deleteSaveData();
            loadEntityCount(ref world, fileData);
            Entity[] entities = world.GetEntities();
            int index = loadEntities(entities, fileData);
            loadMap(ref world, fileData, index);
            return world;
        }

        private static List<LineData> getFileData()
        {
            List<LineData> fileData = new List<LineData>();

            using (StreamReader file = new StreamReader("savegame.txt"))
            {
                do
                {
                    fileData.Add(GetLineData(file.ReadLine()));
                } while (!file.EndOfStream);
            }

            return fileData;
        }

        private static void loadEntityCount(ref World world, List<LineData> fileData)
        {
            int entityCount = int.Parse(fileData[0].FieldValue);
            for(int i = 0; i < entityCount; i++)
            {
                world.CreateEntity();
            }
        }

        private static int loadEntities(Entity[] entities, List<LineData> fileData)
        {
            int index = 1;
            while (fileData[index].FieldName != "Map")
            {
                index = loadEntity(entities, fileData, index);
            };
            return index;
        }

        private static int loadEntity(Entity[] entities, List<LineData> fileData, int index)
        {
            int entityId = int.Parse(fileData[index].FieldValue);
            Entity entity = entities[entityId];
            index++;

            index = loadComponents(entities, fileData, ref entity, index);

            return index;
        }

        private static int loadComponents(Entity[] entities, List<LineData> fileData, ref Entity entity, int index)
        {
            while (fileData[index].FieldName != "Entity" && fileData[index].FieldName != "Map")
            {
                index = loadComponent(entities, fileData, ref entity, index);
            };
            return index;
        }

        private static int loadComponent(Entity[] entities, List<LineData> fileData, ref Entity entity, int index)
        {
            ISaveableComponent component = getComponentByName(fileData[index].FieldValue);
            Type componentType = getComponentType(fileData[index].FieldValue);
            index++;
            
            List<LineData> componentData = new List<LineData>();
            while (fileData[index].FieldName != "Entity"
                        && fileData[index].FieldName != "Map"
                        && fileData[index].FieldName != "Component")
            {
                componentData.Add(fileData[index]);
                index++;
            };

            component.Load(componentData, entities);
            entity.Set(componentType, component);

            return index;
        }

        private static ISaveableComponent getComponentByName(string name)
        {
            ISaveableComponent component = null;
            switch (name)
            {
                case "AreaOfEffect":
                    component = new AreaOfEffect();
                    break;
                case "BlocksTile":
                    component = new BlocksTile();
                    break;
                case "CombatStats":
                    component = new CombatStats();
                    break;
                case "Confusion":
                    component = new Confusion();
                    break;
                case "Consumable":
                    component = new Consumable();
                    break;
                case "InBackpack":
                    component = new InBackpack();
                    break;
                case "InflictsDamage":
                    component = new InflictsDamage();
                    break;
                case "Item":
                    component = new Item();
                    break;
                case "Monster":
                    component = new Monster();
                    break;
                case "Name":
                    component = new Name();
                    break;
                case "Player":
                    component = new Player();
                    break;
                case "Position":
                    component = new Position();
                    break;
                case "ProvidesHealing":
                    component = new ProvidesHealing();
                    break;
                case "Ranged":
                    component = new Ranged();
                    break;
                case "Renderable":
                    component = new Renderable();
                    break;
                case "Viewshed":
                    component = new Viewshed();
                    break;
                case "Equipped":
                    component = new Equipped();
                    break;
                case "Equippable":
                    component = new Equippable();
                    break;
                case "MeleePowerBonus":
                    component = new MeleePowerBonus();
                    break;
                case "DefenseBonus":
                    component = new DefenseBonus();
                    break;
            }
            return component;
        }

        private static Type getComponentType(string name)
        {
            Type type = null;
            switch (name)
            {
                case "AreaOfEffect":
                    type = typeof(AreaOfEffect);
                    break;
                case "BlocksTile":
                    type = typeof(BlocksTile);
                    break;
                case "CombatStats":
                    type = typeof(CombatStats);
                    break;
                case "Confusion":
                    type = typeof(Confusion);
                    break;
                case "Consumable":
                    type = typeof(Consumable);
                    break;
                case "InBackpack":
                    type = typeof(InBackpack);
                    break;
                case "InflictsDamage":
                    type = typeof(InflictsDamage);
                    break;
                case "Item":
                    type = typeof(Item);
                    break;
                case "Monster":
                    type = typeof(Monster);
                    break;
                case "Name":
                    type = typeof(Name);
                    break;
                case "Player":
                    type = typeof(Player);
                    break;
                case "Position":
                    type = typeof(Position);
                    break;
                case "ProvidesHealing":
                    type = typeof(ProvidesHealing);
                    break;
                case "Ranged":
                    type = typeof(Ranged);
                    break;
                case "Renderable":
                    type = typeof(Renderable);
                    break;
                case "Viewshed":
                    type = typeof(Viewshed);
                    break;
                case "Equipped":
                    type = typeof(Equipped);
                    break;
                case "Equippable":
                    type = typeof(Equippable);
                    break;
                case "MeleePowerBonus":
                    type = typeof(MeleePowerBonus);
                    break;
                case "DefenseBonus":
                    type = typeof(DefenseBonus);
                    break;
            }
            return type;
        }

        private static void loadMap(ref World world, List<LineData> fileData, int index)
        {
            int width = int.Parse(fileData[index + 1].FieldValue);
            int height = int.Parse(fileData[index + 2].FieldValue);
            int depth = int.Parse(fileData[index + 3].FieldValue);

            Map.Map map = new Map.Map(width, height, depth);
            map.Load(fileData, index + 4);
            world.SetData(map);
        }


        public static LineData GetLineData(string line)
        {
            string[] lineData = line.Split(':');
            return new LineData() { FieldName = lineData[0], FieldValue = lineData[1] };
        }

        public static Point GetPointFromFieldValue(string fieldValue)
        {
            string[] pointValues = fieldValue.Split(',');
            return new Point(int.Parse(pointValues[0]), int.Parse(pointValues[1]));
        }

        public static Color GetColorFromFieldValue(string fieldValue)
        {
            string[] pointValues = fieldValue.Split(',');
            return new Color(int.Parse(pointValues[0]), int.Parse(pointValues[1]), int.Parse(pointValues[2]), int.Parse(pointValues[3]));
        }
    }
}

using SimpleECS;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Components;
using RogueTutorial.Systems;
using RogueTutorial.Map;
using RogueTutorial.Helpers;
using RogueTutorial.UI;
using RogueTutorial.Interfaces;
using System.IO;
using RogueTutorial.Utils;

namespace RogueTutorial
{
    internal class RootScreen : SadConsole.Console
    {
        World world;
        Query renderablesQuery;
        GameGui gameGui;
        MenuGui menuGui;
        List<ECSSystem> systems;

        Point? mousePosition;

        public RootScreen(int width, int height)
            : base(width, height)
        {
            createWorld();
            initializeSystems();
            initializeGuis();
            this.Surface.IsDirty = true;
        }

        private void createWorld()
        {
            world = World.Create("My World");
            world.SetData(RunState.MainMenu);            
        }

        private void startNewGame(int width, int height)
        {
            clearEntities();

            world.GetData<GameLog>().Entries.Add("Welcome to Rusty Roguelike");

            Map.Map map = MapGenerator.RoomsAndCorridorsGenerator(width, height, 1);
            Random random = new Random();

            world.SetData(map);
            world.SetData(random);

            Spawner.SpawnPlayer(world, map.Rooms.First().Center());

            populateRooms(map);
        }

        private void descendLevel(int width, int height)
        {
            clearEntities(false);

            Map.Map map = MapGenerator.RoomsAndCorridorsGenerator(width, height, world.GetData<Map.Map>().Depth + 1);
            world.SetData(map);
            populateRooms(map);

            Entity player = PlayerFunctions.GetPlayer(world);

            Position playerPosition = player.Get<Position>();
            playerPosition.Point = map.Rooms.First().Center();
            player.Set(playerPosition);

            Viewshed playerView = player.Get<Viewshed>();
            playerView.Dirty = true;
            player.Set(playerView);

            CombatStats playerStats = player.Get<CombatStats>();
            playerStats.Hp = Math.Max(playerStats.Hp, playerStats.MaxHp / 2);
            player.Set(playerStats);

            world.GetData<GameLog>().Entries.Add("You take a moment to heal as you descend to the next level.");
        }

        private void populateRooms(Map.Map map)
        {
            for (int i = 1; i < map.Rooms.Count; i++)
            {
                Spawner.SpawnRoom(world, map.Rooms[i]);
            }
        }

        private void initializeSystems()
        {
            renderablesQuery = world.CreateQuery()
                                .Has<Position>()
                                .Has<Renderable>();

            systems = new List<ECSSystem>()
            {
                new MonsterSystem(world),
                new ItemUseSystem(world),
                new ItemConsumedSystem(world),
                new ItemDropSystem(world),
                new ItemRemoveSystem(world),
                new VisibilitySystem(world),
                new PositionSystem(world),
                new ItemCollectionSystem(world),
                new MeleeCombatSystem(world),
                new DamageSystem(world),
                new DeleteTheDead(world)
            };
        }

        private void initializeGuis()
        {
            gameGui = new GameGui(world);
            menuGui = new MenuGui(world);
            world.SetData(new GameLog() { Entries = new List<string>() });
        }

        public override void Update(TimeSpan delta)
        {
            switch (world.GetData<RunState>())
            {
                case RunState.PreRun:
                    startNewGame(Width, Height - 10);
                    runSystems(delta);
                    world.SetData(RunState.AwaitingInput);
                    break;
                case RunState.NextLevel:
                    descendLevel(Width, Height - 10);
                    runSystems(delta);
                    world.SetData(RunState.AwaitingInput);
                    break;
                case RunState.AwaitingInput:
                case RunState.ShowInventory:
                case RunState.ShowDropItem:
                case RunState.ShowTargeting:
                case RunState.PlayerDeath:
                case RunState.ShowRemoveItem:
                    //Do Nothing
                    break;
                case RunState.PlayerTurn:
                    runSystems(delta);
                    if (world.GetData<RunState>() == RunState.PlayerTurn)
                    {
                        world.SetData(RunState.MonsterTurn);
                    }
                    break;
                case RunState.MonsterTurn:
                    runSystems(delta);
                    if (world.GetData<RunState>() == RunState.MonsterTurn) 
                    { 
                        world.SetData(RunState.AwaitingInput);
                    }
                    Surface.IsDirty = true;
                    break;
                case RunState.SaveGame:
                    saveGameData();
                    world.SetData(RunState.MainMenu);
                    break;
                case RunState.LoadGame:
                    loadGameData();
                    runSystems(delta);
                    Surface.IsDirty = true;
                    world.SetData(RunState.AwaitingInput);
                    break;
            }


            base.Update(delta);
        }

        private void saveGameData()
        {
            SaveGameManager.SaveGame(world);
        }

        private void clearEntities(bool clearAll = true)
        {
            world.CreateQuery().Foreach((Entity entity) =>
            {
                if (clearAll
                    || (!entity.Has<Player>() && !entity.Has<InBackpack>() && !entity.Has<Equipped>())
                    || (entity.Has<InBackpack>() && !entity.Get<InBackpack>().Owner.Has<Player>())
                    || (entity.Has<Equipped>() && !entity.Get<Equipped>().Owner.Has<Player>()))
                {
                    entity.Destroy();
                }
            });
            if (clearAll)
            {
                world.GetData<GameLog>().Entries.Clear();
            }
        }

        private void loadGameData()
        {
            clearEntities();
            world.GetData<GameLog>().Entries.Add("Welcome to Rusty Roguelike");
            SaveGameManager.LoadGame(world);
        }

        private void runSystems(TimeSpan delta)
        {
            foreach (ECSSystem system in systems)
            {
                system.Run(delta);
            }
        }

        
        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            switch (world.GetData<RunState>())
            {
                case RunState.AwaitingInput:
                    processKeyboardMain(keyboard);
                    break;
                case RunState.ShowInventory:
                    processKeyboardInventory(keyboard);
                    break;
                case RunState.ShowDropItem:
                    processKeyboardDropItem(keyboard);
                    break;
                case RunState.ShowTargeting:
                    processKeyboardTargetingSystem(keyboard);
                    break;
                case RunState.MainMenu:
                    processKeyboardMainMenu(keyboard);
                    break;
                case RunState.PlayerDeath:
                    processKeyboardPlayerDead(keyboard);
                    break;
                case RunState.ShowRemoveItem:
                    processKeyboardRemoveItem(keyboard);
                    break;
            }

            return true;
        }

        private void processKeyboardMain(Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(Keys.Left) || keyboard.IsKeyPressed(Keys.NumPad4) || keyboard.IsKeyPressed(Keys.H))
            {
                Surface.IsDirty = PlayerFunctions.TryMovePlayer(world, Direction.Left);
                world.SetData(RunState.PlayerTurn);
            }
            if (keyboard.IsKeyPressed(Keys.Right) || keyboard.IsKeyPressed(Keys.NumPad6) || keyboard.IsKeyPressed(Keys.L))
            {
                Surface.IsDirty = PlayerFunctions.TryMovePlayer(world, Direction.Right);
                world.SetData(RunState.PlayerTurn);
            }
            if (keyboard.IsKeyPressed(Keys.Up) || keyboard.IsKeyPressed(Keys.NumPad8) || keyboard.IsKeyPressed(Keys.K))
            {
                Surface.IsDirty = PlayerFunctions.TryMovePlayer(world, Direction.Up);
                world.SetData(RunState.PlayerTurn);
            }
            if (keyboard.IsKeyPressed(Keys.Down) || keyboard.IsKeyPressed(Keys.NumPad2) || keyboard.IsKeyPressed(Keys.J))
            {
                Surface.IsDirty = PlayerFunctions.TryMovePlayer(world, Direction.Down);
                world.SetData(RunState.PlayerTurn);
            }
            if (keyboard.IsKeyPressed(Keys.NumPad9) || keyboard.IsKeyPressed(Keys.Y))
            {
                Surface.IsDirty = PlayerFunctions.TryMovePlayer(world, Direction.UpRight);
                world.SetData(RunState.PlayerTurn);
            }
            if (keyboard.IsKeyPressed(Keys.NumPad7) || keyboard.IsKeyPressed(Keys.U))
            {
                Surface.IsDirty = PlayerFunctions.TryMovePlayer(world, Direction.UpLeft);
                world.SetData(RunState.PlayerTurn);
            }
            if (keyboard.IsKeyPressed(Keys.NumPad3) || keyboard.IsKeyPressed(Keys.N))
            {
                Surface.IsDirty = PlayerFunctions.TryMovePlayer(world, Direction.DownRight);
                world.SetData(RunState.PlayerTurn);
            }
            if (keyboard.IsKeyPressed(Keys.NumPad1) || keyboard.IsKeyPressed(Keys.B))
            {
                Surface.IsDirty = PlayerFunctions.TryMovePlayer(world, Direction.DownLeft);
                world.SetData(RunState.PlayerTurn);
            }

            if (keyboard.IsKeyPressed(Keys.G))
            {
                switch(ItemCollectionSystem.GetItem(world, PlayerFunctions.GetPlayer(world)))
                {
                    case 0:
                        //Do nothing
                        break;
                    case 1:
                        Surface.IsDirty = true;
                        world.SetData(RunState.PlayerTurn);
                        break;
                    case 2:
                        Surface.IsDirty = true;
                        break;
                }
            }
            if (keyboard.IsKeyPressed(Keys.I))
            {
                Surface.IsDirty = true;
                world.SetData(RunState.ShowInventory);
            }
            if (keyboard.IsKeyPressed(Keys.D))
            {
                Surface.IsDirty = true;
                world.SetData(RunState.ShowDropItem);
            }
            if (keyboard.IsKeyPressed(Keys.R))
            {
                Surface.IsDirty = true;
                world.SetData(RunState.ShowRemoveItem);
            }
            if (keyboard.IsKeyPressed(Keys.OemPeriod))
            {
                if (PlayerFunctions.TryPlayerDescend(world))
                {
                    world.SetData(RunState.NextLevel);
                }
                Surface.IsDirty = true;
            }
            if(keyboard.IsKeyPressed(Keys.Space) || keyboard.IsKeyPressed(Keys.NumPad5))
            {
                PlayerFunctions.SkipPlayerTurn(world);
                world.SetData(RunState.PlayerTurn);
                Surface.IsDirty = true;
            }

            if (keyboard.IsKeyPressed(Keys.Escape) || keyboard.IsKeyPressed(Keys.Q))
            {
                Surface.IsDirty = true;
                world.SetData(RunState.SaveGame);
            }
        }

        private void processKeyboardInventory(Keyboard keyboard)
        {
            gameGui.ProcessKeyboardInventory(keyboard, Surface);
        }

        private void processKeyboardDropItem(Keyboard keyboard)
        {
            gameGui.ProcessKeyboardDropItem(keyboard, Surface);
        }

        private void processKeyboardRemoveItem(Keyboard keyboard)
        {
            gameGui.ProcessKeyboardRemoveItem(keyboard, Surface);
        }

        private void processKeyboardTargetingSystem(Keyboard keyboard)
        {
            gameGui.ProcessKeyboardTargetingSystem(keyboard, Surface);
        }

        private void processKeyboardMainMenu(Keyboard keyboard)
        {
            menuGui.ProcessKeyboard(keyboard, Surface);
        }

        private void processKeyboardPlayerDead(Keyboard keyboard)
        {
            if (keyboard.HasKeysPressed)
            {
                clearEntities();
                world.SetData(RunState.MainMenu);
                Surface.IsDirty = true;
            }
        }

        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            if (state.Mouse.IsMouseOverScreenObjectSurface(this))
            {
                if(mousePosition != state.CellPosition)
                {
                    mousePosition = state.CellPosition;
                    Surface.IsDirty = true;
                }

                if(world.GetData<RunState>() == RunState.ShowTargeting)
                {
                    gameGui.ProcessMouseTargetingSystem(state);
                }
            }
            else
            {
                if(mousePosition != null)
                {
                    Surface.IsDirty = true;
                }
                mousePosition = null;
            }
            return base.ProcessMouse(state);
        }

        public override void Render(TimeSpan delta)
        {
            if (Surface.IsDirty)
            {
                Surface.Clear();
                switch (world.GetData<RunState>())
                {
                    case RunState.MainMenu:
                        menuGui.Render(Surface, mousePosition);
                        break;
                    case RunState.PlayerDeath:
                        renderGameOver();
                        break;
                    default:
                        renderGame();
                        break;
                }

            }
            base.Render(delta);
        }

        private void renderGame()
        {
            Map.Map map = world.GetData<Map.Map>();
            Entity player = PlayerFunctions.GetPlayer(world);
            Viewshed playerVisibility = player.Get<Viewshed>();

            renderMap(map, playerVisibility);
            renderEntities(playerVisibility);
            gameGui.Render(Surface, mousePosition);
        }

        private void renderMap(Map.Map map, Viewshed playerVisibility)
        {
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    if (playerVisibility.VisibleTiles.Any(a => a.X == i && a.Y == j))
                    {
                        switch (map.GetMapCell(i, j))
                        {
                            case TileType.Floor:
                                TileGlyphs.FloorVisible.CopyAppearanceTo(Surface[i, j]);
                                break;
                            case TileType.Wall:
                                TileGlyphs.WallVisible.CopyAppearanceTo(Surface[i, j]);
                                break;
                            case TileType.DownStairs:
                                TileGlyphs.DownStairsVisible.CopyAppearanceTo(Surface[i, j]);
                                break;
                        }
                    }
                    else if (map.IsMapCellExplored(i, j))
                    {
                        switch (map.GetMapCell(i, j))
                        {
                            case TileType.Floor:
                                TileGlyphs.Floor.CopyAppearanceTo(Surface[i, j]);
                                break;
                            case TileType.Wall:
                                TileGlyphs.Wall.CopyAppearanceTo(Surface[i, j]);
                                break;
                            case TileType.DownStairs:
                                TileGlyphs.DownStairs.CopyAppearanceTo(Surface[i, j]);
                                break;
                        }
                    }
                }
            }
        }

        private void renderEntities(Viewshed playerVisibility)
        {
            foreach(Entity entity in renderablesQuery.GetEntities().OrderBy(a => a.Get<Renderable>().RenderOrder))
            {
                Point point = entity.Get<Position>().Point;
                if(playerVisibility.VisibleTiles.Any(a => a == point))
                {
                    entity.Get<Renderable>().Glyph.CopyAppearanceTo(Surface[point]);
                }
            }
        }

        private void renderGameOver()
        {
            Surface.Print(Surface.Width / 2 - 11, 15, "Your journey has ended!", Color.Yellow, Color.Black);
            Surface.Print(Surface.Width / 2 - 23, 17, "One day, we'll tell you all about how you did.", Color.White, Color.Black);
            Surface.Print(Surface.Width / 2 - 21, 18, "That day, sadly, is not in this chapter...", Color.White, Color.Black);
            Surface.Print(Surface.Width / 2 - 20, 20, "Press any key to return to the main menu.", Color.Magenta, Color.Black);
        }
    }
}

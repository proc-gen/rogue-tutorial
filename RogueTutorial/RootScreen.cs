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
        Query playerQuery;
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
            if(world.EntityCount > 0)
            {
                foreach(Entity entity in world.GetEntities())
                {
                    entity.Destroy();
                }
            }

            Map.Map map = MapGenerator.RoomsAndCorridorsGenerator(width, height);
            Random random = new Random();

            world.SetData(map);
            world.SetData(random);

            Spawner.SpawnPlayer(world, map.Rooms.First().Center());

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

            playerQuery = world.CreateQuery().Has<Player>();

            systems = new List<ECSSystem>();

            systems.Add(new MonsterSystem(world
                                            , world.CreateQuery()
                                                .Has<Position>()
                                                .Has<Viewshed>()
                                                .Has<Monster>()
                                            , playerQuery));
            systems.Add(new ItemUseSystem(world
                                            , world.CreateQuery()
                                                .Has<WantsToUseItem>()));
            systems.Add(new ItemConsumedSystem(world
                                            , world.CreateQuery()
                                                .Has<WantsToUseItem>()));
            systems.Add(new ItemDropSystem(world
                                            , world.CreateQuery()
                                                .Has<WantsToDropItem>()));
            systems.Add(new VisibilitySystem(world
                                                , world.CreateQuery()
                                                    .Has<Position>()
                                                    .Has<Viewshed>()
                                                , playerQuery));
            systems.Add(new PositionSystem(world
                                            , world.CreateQuery()
                                                .Has<Position>()));
            systems.Add(new ItemCollectionSystem(world
                                                    , world.CreateQuery()
                                                        .Has<WantsToPickupItem>()));
            systems.Add(new MeleeCombatSystem(world
                                                , world.CreateQuery()
                                                    .Has<WantsToMelee>()));
            systems.Add(new DamageSystem(world
                                            , world.CreateQuery()
                                                .Has<SufferDamage>()));
            systems.Add(new DeleteTheDead(world
                                            , world.CreateQuery()
                                                .Has<CombatStats>()));
            
        }

        private void initializeGuis()
        {
            gameGui = new GameGui(world, playerQuery);
            menuGui = new MenuGui(world);
            world.SetData(new GameLog() { Entries = new List<string>() { "Welcome to Rusty Roguelike" } });
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
                case RunState.AwaitingInput:
                case RunState.ShowInventory:
                case RunState.ShowDropItem:
                case RunState.ShowTargeting:
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
                    world.SetData(RunState.AwaitingInput);
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

        private void loadGameData()
        {
            if (world.EntityCount > 0)
            {
                foreach (Entity entity in world.GetEntities())
                {
                    entity.Destroy();
                }
            }
            SaveGameManager.LoadGame(world);
        }

        private void runSystems(TimeSpan delta)
        {
            foreach (ECSSystem system in systems)
            {
                system.Run(delta);
            }
        }

        private bool tryMovePlayer(Direction direction)
        {
            bool retVal = false;
            var map = world.GetData<Map.Map>();

            playerQuery.Foreach((Entity player, ref Position position, ref Viewshed visibility, ref Name name) =>
            {
                Point newPoint = position.Point.Add(direction);
                if(map.IsCellWalkable(newPoint.X,newPoint.Y))
                {
                    position.Point = newPoint;
                    visibility.Dirty = true;

                    map.SetCellWalkable(position.PreviousPoint.X, position.PreviousPoint.Y, true);
                    map.SetCellWalkable(position.Point.X, position.Point.Y, false);
                    position.Dirty = false;

                    retVal = true;
                }
                else
                {
                    List<Entity> cellEntities = map.GetCellEntities(newPoint);
                    if(cellEntities.Count > 0)
                    {
                        Entity monster = cellEntities.Where(a => a.Has<Monster>()).FirstOrDefault();
                        if(monster)
                        {
                            player.Set(new WantsToMelee() { Target = monster});
                            retVal = true;
                        }
                    }
                }
            });

            return retVal;
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
            }

            return true;
        }

        private void processKeyboardMain(Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(Keys.Left) || keyboard.IsKeyPressed(Keys.NumPad4) || keyboard.IsKeyPressed(Keys.H))
            {
                Surface.IsDirty = tryMovePlayer(Direction.Left);
                world.SetData(RunState.PlayerTurn);
            }
            if (keyboard.IsKeyPressed(Keys.Right) || keyboard.IsKeyPressed(Keys.NumPad6) || keyboard.IsKeyPressed(Keys.L))
            {
                Surface.IsDirty = tryMovePlayer(Direction.Right);
                world.SetData(RunState.PlayerTurn);
            }
            if (keyboard.IsKeyPressed(Keys.Up) || keyboard.IsKeyPressed(Keys.NumPad8) || keyboard.IsKeyPressed(Keys.K))
            {
                Surface.IsDirty = tryMovePlayer(Direction.Up);
                world.SetData(RunState.PlayerTurn);
            }
            if (keyboard.IsKeyPressed(Keys.Down) || keyboard.IsKeyPressed(Keys.NumPad2) || keyboard.IsKeyPressed(Keys.J))
            {
                Surface.IsDirty = tryMovePlayer(Direction.Down);
                world.SetData(RunState.PlayerTurn);
            }
            if (keyboard.IsKeyPressed(Keys.NumPad9) || keyboard.IsKeyPressed(Keys.Y))
            {
                Surface.IsDirty = tryMovePlayer(Direction.UpRight);
                world.SetData(RunState.PlayerTurn);
            }
            if (keyboard.IsKeyPressed(Keys.NumPad7) || keyboard.IsKeyPressed(Keys.U))
            {
                Surface.IsDirty = tryMovePlayer(Direction.UpLeft);
                world.SetData(RunState.PlayerTurn);
            }
            if (keyboard.IsKeyPressed(Keys.NumPad3) || keyboard.IsKeyPressed(Keys.N))
            {
                Surface.IsDirty = tryMovePlayer(Direction.DownRight);
                world.SetData(RunState.PlayerTurn);
            }
            if (keyboard.IsKeyPressed(Keys.NumPad1) || keyboard.IsKeyPressed(Keys.B))
            {
                Surface.IsDirty = tryMovePlayer(Direction.DownLeft);
                world.SetData(RunState.PlayerTurn);
            }

            if (keyboard.IsKeyPressed(Keys.G))
            {
                switch(ItemCollectionSystem.GetItem(world, playerQuery.GetEntities()[0]))
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
            gameGui.ProcessKeyboardInventory(keyboard, Surface);
        }

        private void processKeyboardTargetingSystem(Keyboard keyboard)
        {
            gameGui.ProcessKeyboardTargetingSystem(keyboard, Surface);
        }

        private void processKeyboardMainMenu(Keyboard keyboard)
        {
            menuGui.ProcessKeyboard(keyboard, Surface);
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
                if (world.GetData<RunState>() == RunState.MainMenu)
                {
                    menuGui.Render(Surface, mousePosition);
                }
                else
                {
                    Map.Map map = world.GetData<Map.Map>();
                    Entity player = playerQuery.GetEntities()[0];
                    Viewshed playerVisibility = player.Get<Viewshed>();

                    renderMap(map, playerVisibility);
                    renderEntities(playerVisibility);

                    gameGui.Render(Surface, mousePosition);
                }
            }
            base.Render(delta);
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
    }
}

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
            world.CreateQuery().Foreach((Entity entity) =>
            {
                bool remove = false;
                if (!entity.Has<Player>())
                {
                    if (entity.Has<InBackpack>())
                    {
                        if (!entity.Get<InBackpack>().Owner.Has<Player>())
                        {
                            remove = true;
                        }
                    }
                    else
                    {
                        remove = true;
                    }
                }
                if (remove)
                {
                    entity.Destroy();
                }
            });

            Map.Map map = MapGenerator.RoomsAndCorridorsGenerator(width, height, world.GetData<Map.Map>().Depth + 1);
            world.SetData(map);
            populateRooms(map);

            Entity player = playerQuery.GetEntities().First();

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

        private void clearEntities()
        {
            if (world.EntityCount > 0)
            {
                foreach (Entity entity in world.GetEntities())
                {
                    entity.Destroy();
                }
            }
            world.GetData<GameLog>().Entries.Clear();
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

        private bool tryMovePlayer(Direction direction)
        {
            bool retVal = false;
            Map.Map map = world.GetData<Map.Map>();

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

        private bool tryPlayerDescend()
        {
            bool retVal = false;
            
            playerQuery.Foreach((in Map.Map map, in GameLog log, Entity player, ref Position position) =>
            {
                if(map.GetMapCell(position.Point.X, position.Point.Y) == TileType.DownStairs)
                {
                    retVal = true;
                }
                else
                {
                    log.Entries.Add("There is no way down from here.");
                }
            });
            return retVal;
        }

        private void skipPlayerTurn()
        {
            playerQuery.Foreach((in Map.Map map, Entity player, ref Position position, ref Viewshed viewshed, ref CombatStats combatStats) =>
            {
                bool canHeal = true;

                foreach(Point visible in viewshed.VisibleTiles)
                {
                    if(map.GetCellEntities(visible).Any(a => a.Has<Monster>()))
                    {
                        canHeal = false;
                    }
                }

                if (canHeal)
                {
                    combatStats.Hp = Math.Min(combatStats.Hp + 1, combatStats.MaxHp);
                }
            });

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
            if (keyboard.IsKeyPressed(Keys.OemPeriod))
            {
                if (tryPlayerDescend())
                {
                    world.SetData(RunState.NextLevel);
                }
                Surface.IsDirty = true;
            }
            if(keyboard.IsKeyPressed(Keys.Space) || keyboard.IsKeyPressed(Keys.NumPad5))
            {
                skipPlayerTurn();
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
    }
}

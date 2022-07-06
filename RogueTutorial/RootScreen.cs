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

namespace RogueTutorial
{
    internal class RootScreen : SadConsole.Console
    {
        World world;
        Query renderablesQuery;
        Query playerQuery;
        Gui gui;
        List<Systems.ECSSystem> systems;

        Point? mousePosition;

        public RootScreen(int width, int height)
            : base(width, height)
        {
            createWorld(width, height - 10);
            initializeSystems();
            initializeGui();
            this.Surface.IsDirty = true;
        }

        private void createWorld(int width, int height)
        {
            world = World.Create("My World");
            Map.Map map = MapGenerator.RoomsAndCorridorsGenerator(width, height);

            world.SetData(map);
            world.SetData(RunState.PreRun);

            world.CreateEntity(new Position() { Point = map.Rooms.First().Center() }
                                , new Renderable() { Glyph = new ColoredGlyph(Color.Yellow, Color.Black, '@') }
                                , new Player()
                                , new Viewshed() { VisibleTiles = new List<Point>(), Range = 8, Dirty = true }
                                , new Name() { EntityName = "Player"}
                                , new BlocksTile()
                                , new CombatStats() { MaxHp = 30, Hp = 30, Defense = 2, Power = 5});

            if (map.Rooms.Count > 1)
            {
                Random random = new Random();
                for (int i = 1; i < map.Rooms.Count; i++)
                {
                    int monsterId = random.Next(2);
                    world.CreateEntity(new Position() { Point = map.Rooms[i].Center() }
                                        , new Renderable() { Glyph = new ColoredGlyph(Color.Red, Color.Black, monsterId == 1 ? 'g' : 'o')}
                                        , new Viewshed() { VisibleTiles = new List<Point>(), Range = 8, Dirty = true }
                                        , new Monster()
                                        , new Name () { EntityName = (monsterId == 1 ? "Goblin" : "Orc") + " #" + i.ToString()}
                                        , new BlocksTile()
                                        , new CombatStats() { MaxHp = 16, Hp = 16, Defense = 1, Power = 4 });
                }
            }
        }

        private void initializeSystems()
        {
            renderablesQuery = world.CreateQuery()
                                .Has<Position>()
                                .Has<Renderable>();

            playerQuery = world.CreateQuery().Has<Player>();

            systems = new List<ECSSystem>();

            
            systems.Add(new VisibilitySystem(world
                                                , world.CreateQuery()
                                                    .Has<Position>()
                                                    .Has<Viewshed>()
                                                , playerQuery));
            systems.Add(new MonsterSystem(world
                                            , world.CreateQuery()
                                                .Has<Position>()
                                                .Has<Viewshed>()
                                                .Has<Monster>()
                                            , playerQuery));
            systems.Add(new PositionSystem(world
                                            , world.CreateQuery()
                                                .Has<Position>()));
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

        private void initializeGui()
        {
            gui = new Gui(world, playerQuery);
            world.SetData(new GameLog() { Entries = new List<string>() { "Welcome to Rusty Roguelike" } });
        }

        public override void Update(TimeSpan delta)
        {
            switch (world.GetData<RunState>())
            {
                case RunState.PreRun:
                    runSystems(delta);
                    world.SetData(RunState.AwaitingInput);
                    break;
                case RunState.AwaitingInput:
                    //Do Nothing
                    break;
                case RunState.PlayerTurn:
                    runSystems(delta);
                    world.SetData(RunState.MonsterTurn);
                    break;
                case RunState.MonsterTurn:
                    runSystems(delta);
                    world.SetData(RunState.AwaitingInput);
                    break;
            }


            base.Update(delta);
        }

        private void runSystems(TimeSpan delta)
        {
            foreach (Systems.ECSSystem system in systems)
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
            if (world.GetData<RunState>() == RunState.AwaitingInput)
            {
                if (keyboard.IsKeyPressed(Keys.Left) || keyboard.IsKeyPressed(Keys.NumPad4) || keyboard.IsKeyPressed(Keys.H))
                {
                    this.Surface.IsDirty = tryMovePlayer(Direction.Left);
                }
                if (keyboard.IsKeyPressed(Keys.Right) || keyboard.IsKeyPressed(Keys.NumPad6) || keyboard.IsKeyPressed(Keys.L))
                {
                    this.Surface.IsDirty = tryMovePlayer(Direction.Right);
                }
                if (keyboard.IsKeyPressed(Keys.Up) || keyboard.IsKeyPressed(Keys.NumPad8) || keyboard.IsKeyPressed(Keys.K))
                {
                    this.Surface.IsDirty = tryMovePlayer(Direction.Up);
                }
                if (keyboard.IsKeyPressed(Keys.Down) || keyboard.IsKeyPressed(Keys.NumPad2) || keyboard.IsKeyPressed(Keys.J))
                {
                    this.Surface.IsDirty = tryMovePlayer(Direction.Down);
                }
                if (keyboard.IsKeyPressed(Keys.NumPad9) || keyboard.IsKeyPressed(Keys.Y))
                {
                    this.Surface.IsDirty = tryMovePlayer(Direction.UpRight);
                }
                if (keyboard.IsKeyPressed(Keys.NumPad7) || keyboard.IsKeyPressed(Keys.U))
                {
                    this.Surface.IsDirty = tryMovePlayer(Direction.UpLeft);
                }
                if (keyboard.IsKeyPressed(Keys.NumPad3) || keyboard.IsKeyPressed(Keys.N))
                {
                    this.Surface.IsDirty = tryMovePlayer(Direction.DownRight);
                }
                if (keyboard.IsKeyPressed(Keys.NumPad1) || keyboard.IsKeyPressed(Keys.B))
                {
                    this.Surface.IsDirty = tryMovePlayer(Direction.DownLeft);
                }

                if (keyboard.IsKeyPressed(Keys.Escape) || keyboard.IsKeyPressed(Keys.Q))
                {
                    Game.Instance.MonoGameInstance.Exit();
                }

                if (this.Surface.IsDirty)
                {
                    world.SetData(RunState.PlayerTurn);
                }
            }

            return true;
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
                Map.Map map = world.GetData<Map.Map>();
                Entity player = playerQuery.GetEntities()[0];
                Viewshed playerVisibility = player.Get<Viewshed>();

                renderMap(map, playerVisibility);
                renderEntities(playerVisibility);                

                gui.Render(Surface, mousePosition);
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
            renderablesQuery.Foreach((ref Position position, ref Renderable renderable) =>
            {
                Point point = position.Point;
                if (playerVisibility.VisibleTiles.Any(a => a.X == point.X && a.Y == point.Y))
                {
                    renderable.Glyph.CopyAppearanceTo(Surface[position.Point]);
                }
            });
        }
    }
}

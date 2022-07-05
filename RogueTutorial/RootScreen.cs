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

namespace RogueTutorial
{
    internal class RootScreen : SadConsole.Console
    {
        World world;
        Query renderablesQuery;
        Query playerQuery;
        List<Systems.ECSSystem> systems;

        public RootScreen(int width, int height)
            : base(width, height)
        {
            createWorld(width, height);
            initializeSystems();
            
            this.Surface.IsDirty = true;
        }

        private void createWorld(int width, int height)
        {
            world = World.Create("My World");
            Map.Map map = MapGenerator.RoomsAndCorridorsGenerator(width, height);

            world.SetData(map);
            world.SetData(RunState.Running);

            world.CreateEntity(new Position() { Point = map.Rooms.First().Center() }
                                , new Renderable() { Glyph = new ColoredGlyph(Color.Yellow, Color.Black, '@') }
                                , new Player()
                                , new Viewshed() { VisibleTiles = new List<Point>(), Range = 8, Dirty = true }
                                , new Name() { EntityName = "Player"}
                                , new BlocksTile());

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
                                        , new BlocksTile());
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
                                                .Has<Position>()
                                                .Has<BlocksTile>()));
            
        }

        public override void Update(TimeSpan delta)
        {
            if (world.GetData<RunState>() == RunState.Running)
            {
                foreach (Systems.ECSSystem system in systems)
                {
                    system.Run(delta);
                }
                world.SetData(RunState.Paused);
            }

            base.Update(delta);
        }

        private bool tryMovePlayer(Direction direction)
        {
            bool retVal = false;
            var map = world.GetData<Map.Map>();

            playerQuery.Foreach((ref Position position, ref Viewshed visibility, ref Name name) =>
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
            });

            return retVal;
        }

        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            if (world.GetData<RunState>() == RunState.Paused)
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
                    world.SetData(RunState.Running);
                }
            }

            return true;
        }

        public override void Render(TimeSpan delta)
        {
            if (this.Surface.IsDirty)
            {
                this.Surface.Clear();
                Map.Map map = world.GetData<Map.Map>();
                Entity player = playerQuery.GetEntities()[0];
                Viewshed playerVisibility = player.Get<Viewshed>();

                for(int i = 0; i < map.Width; i++)
                {
                    for (int j = 0; j < map.Height; j++)
                    {
                        if (playerVisibility.VisibleTiles.Any(a => a.X == i && a.Y == j)) 
                        { 
                            switch (map.GetMapCell(i, j))
                            {
                                case TileType.Floor:
                                    TileGlyphs.FloorVisible.CopyAppearanceTo(this.Surface[i, j]);
                                    break;
                                case TileType.Wall:
                                    TileGlyphs.WallVisible.CopyAppearanceTo(this.Surface[i, j]);
                                    break;
                            }
                        }
                        else if(map.IsMapCellExplored(i, j))
                        {
                            switch (map.GetMapCell(i, j))
                            {
                                case TileType.Floor:
                                    TileGlyphs.Floor.CopyAppearanceTo(this.Surface[i, j]);
                                    break;
                                case TileType.Wall:
                                    TileGlyphs.Wall.CopyAppearanceTo(this.Surface[i, j]);
                                    break;
                            }
                        }
                    }
                }
                renderablesQuery.Foreach((ref Position position, ref Renderable renderable) =>
                {
                    Point point = position.Point;
                    if (playerVisibility.VisibleTiles.Any(a => a.X == point.X && a.Y == point.Y))
                    {
                        renderable.Glyph.CopyAppearanceTo(this.Surface[position.Point]);
                    }
                });
            }
            base.Render(delta);
        }
    }
}

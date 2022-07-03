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

namespace RogueTutorial
{
    internal class RootScreen : SadConsole.Console
    {
        World world;
        Query renderablesQuery;
        Query playerQuery;
        List<Systems.System> systems;

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

            world.CreateEntity(new Position() { Point = map.Rooms.First().Center() }
                                , new Renderable() { Glyph = new ColoredGlyph(Color.Yellow, Color.Black, '@') }
                                , new Player()
                                , new Viewshed() { VisibleTiles = new List<Point>(), Range = 9 });
        }

        private void initializeSystems()
        {
            renderablesQuery = world.CreateQuery()
                                .Has<Position>()
                                .Has<Renderable>();

            playerQuery = world.CreateQuery().Has<Player>();

            systems = new List<Systems.System>();

            systems.Add(new VisibilitySystem(world
                                                , world.CreateQuery()
                                                    .Has<Position>()
                                                    .Has<Viewshed>()
                                                , playerQuery));
        }

        public override void Update(TimeSpan delta)
        {
            foreach(Systems.System system in systems)
            {
                system.Run(delta);
            }
            base.Update(delta);
        }

        private bool tryMovePlayer(Direction direction)
        {
            bool retVal = false;
            var map = world.GetData<Map.Map>();

            playerQuery.Foreach((ref Position position) =>
            {
                Point newPoint = position.Point.Add(direction);
                if(map.GetMapCell(newPoint.X,newPoint.Y) != TileType.Wall)
                {
                    position.Point = newPoint;
                    retVal = true;
                }
            });

            return retVal;
        }

        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            bool handled = false;
            if (keyboard.IsKeyPressed(Keys.Left) || keyboard.IsKeyPressed(Keys.NumPad4) || keyboard.IsKeyPressed(Keys.H))
            {
                this.Surface.IsDirty = tryMovePlayer(Direction.Left);
                handled = this.Surface.IsDirty;
            }
            if (keyboard.IsKeyPressed(Keys.Right) || keyboard.IsKeyPressed(Keys.NumPad6) || keyboard.IsKeyPressed(Keys.L))
            {
                this.Surface.IsDirty = tryMovePlayer(Direction.Right);
                handled = this.Surface.IsDirty;
            }
            if (keyboard.IsKeyPressed(Keys.Up) || keyboard.IsKeyPressed(Keys.NumPad8) || keyboard.IsKeyPressed(Keys.K))
            {
                this.Surface.IsDirty = tryMovePlayer(Direction.Up);
                handled = this.Surface.IsDirty;
            }
            if (keyboard.IsKeyPressed(Keys.Down) || keyboard.IsKeyPressed(Keys.NumPad2) || keyboard.IsKeyPressed(Keys.J))
            {
                this.Surface.IsDirty = tryMovePlayer(Direction.Down);
                handled = this.Surface.IsDirty;
            }
            if (keyboard.IsKeyPressed(Keys.Escape) || keyboard.IsKeyPressed(Keys.Q)) 
            {
                Game.Instance.MonoGameInstance.Exit();
                handled = true;
            }
            return handled;
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

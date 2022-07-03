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

        public RootScreen(int width, int height)
            : base(width, height)
        {
            this.UseMouse = false;

            world = World.Create("My World");
            Map.Map map = MapGenerator.RoomsAndCorridorsGenerator(width, height);

            world.SetData(map); 
            
            world.CreateEntity(new Position() { Point = map.Rooms.First().Center() }
                                , new Renderable() {Glyph = new ColoredGlyph(Color.White, Color.Black, '@')}
                                , new Player());

            

            renderablesQuery = world.CreateQuery()
                                .Has<Position>()
                                .Has<Renderable>();

            playerQuery = world.CreateQuery().Has<Player>();
            this.Surface.IsDirty = true;
        }


        public override void Update(TimeSpan delta)
        {
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
                var map = world.GetData<Map.Map>();
                for(int i = 0; i < map.Width; i++)
                {
                    for(int j = 0; j < map.Height; j++)
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
                renderablesQuery.Foreach((ref Position position, ref Renderable renderable) =>
                {
                    renderable.Glyph.CopyAppearanceTo(this.Surface[position.Point]);
                });
            }
            base.Render(delta);
        }
    }
}

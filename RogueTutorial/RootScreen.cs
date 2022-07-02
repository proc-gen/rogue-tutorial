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
            world.CreateEntity(new Position() { Point = new Point(this.Width/2, this.Height/2) }
                                , new Renderable() {Glyph = new ColoredGlyph(Color.White, Color.Black, '@')}
                                , new Player());

            world.SetData(MapGenerator.DefaultGenerator(width, height));

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
            if (keyboard.IsKeyPressed(Keys.Left))
            {
                this.Surface.IsDirty = tryMovePlayer(Direction.Left);
                handled = this.Surface.IsDirty;
            }
            if (keyboard.IsKeyPressed(Keys.Right))
            {
                this.Surface.IsDirty = tryMovePlayer(Direction.Right);
                handled = this.Surface.IsDirty;
            }
            if (keyboard.IsKeyPressed(Keys.Up))
            {
                this.Surface.IsDirty = tryMovePlayer(Direction.Up);
                handled = this.Surface.IsDirty;
            }
            if (keyboard.IsKeyPressed(Keys.Down))
            {
                this.Surface.IsDirty = tryMovePlayer(Direction.Down);
                handled = this.Surface.IsDirty;
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

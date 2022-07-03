using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueSharp;

using Point = SadRogue.Primitives.Point;

namespace RogueTutorial.Map
{
    internal class Map
    {
        private TileType[] _mapGrid;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public List<Rectangle> Rooms { get; set; }

        private IMap map { get; set; }

        public Map(int width, int height)
        {
            Height = height;
            Width = width;
            _mapGrid = new TileType[width * height];
            Rooms = new List<Rectangle>();
            map = new RogueSharp.Map(width, height);
        }

        public TileType GetMapCell(int x, int y)
        {
            return _mapGrid[y * Width + x];
        }

        public void SetMapCell(int x, int y, TileType tileType)
        {
            _mapGrid[y * Width + x] = tileType;
            map.SetCellProperties(x, y, tileType == TileType.Floor, tileType == TileType.Floor, false);
        }

        public List<Point> ComputeFOV(int x, int y, int radius, bool lightWalls, bool updateMap)
        {
            var cells = map.ComputeFov(x, y, radius, lightWalls);
            if (updateMap)
            {
                foreach (var cell in cells)
                {
                    updateCellVisibility(cell.X, cell.Y, true);
                }
            }

            return cells.Select(cell => new Point(cell.X, cell.Y)).ToList();
        }

        private void updateCellVisibility(int x, int y, bool visibility)
        {
            var tileType = GetMapCell(x, y);
            map.SetCellProperties(x, y, tileType == TileType.Floor, tileType == TileType.Floor, visibility);
        }

        public bool IsMapCellExplored(int x, int y)
        {
            return map.IsExplored(x, y);
        }
    }
}

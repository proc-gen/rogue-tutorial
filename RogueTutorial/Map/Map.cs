using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Map
{
    internal class Map
    {
        private TileType[] _mapGrid;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public List<Rectangle> Rooms { get; set; }

        public Map(int width, int height)
        {
            Height = height;
            Width = width;
            _mapGrid = new TileType[width * height];
            Rooms = new List<Rectangle>();
        }

        public TileType GetMapCell(int x, int y)
        {
            return _mapGrid[y * Width + x];
        }

        public void SetMapCell(int x, int y, TileType tileType)
        {
            _mapGrid[y * Width + x] = tileType;
        }
    }
}

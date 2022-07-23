using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using SadRogue.Primitives;

namespace RogueTutorial.Map
{
    public enum TileType
    {
        Wall = '#',
        Floor = '.',
        DownStairs = 31
    }

    public static class TileTypeExtensions
    {
        public static TileType GetTileTypeFromString(string tileType)
        {
            switch (tileType)
            {
                case "Wall":
                    return TileType.Wall;
                    break;
                case "Floor":
                    return TileType.Floor;
                    break;
                case "DownStairs":
                    return TileType.DownStairs;
                    break;
                default:
                    return TileType.Wall;
                    break;
            }
        }
    }

    public static class TileGlyphs
    {

        public static ColoredGlyph Wall = new ColoredGlyph(Color.Gray, Color.Black, (char)TileType.Wall);
        public static ColoredGlyph WallVisible = new ColoredGlyph(Color.LimeGreen, Color.Black, (char)TileType.Wall);

        public static ColoredGlyph Floor = new ColoredGlyph(Color.Gray, Color.Black, (char)TileType.Floor);
        public static ColoredGlyph FloorVisible = new ColoredGlyph(Color.Green, Color.Black, (char)TileType.Floor);

        public static ColoredGlyph DownStairs = new ColoredGlyph(Color.Gray, Color.Black, (char)TileType.DownStairs);
        public static ColoredGlyph DownStairsVisible = new ColoredGlyph(Color.Cyan, Color.Black, (char)TileType.DownStairs);

        private static ColoredGlyph[] Walls = new ColoredGlyph[16]
        {
            new ColoredGlyph(Color.Gray, Color.Black, 9),
            new ColoredGlyph(Color.Gray, Color.Black, 186),
            new ColoredGlyph(Color.Gray, Color.Black, 186),
            new ColoredGlyph(Color.Gray, Color.Black, 186),
            new ColoredGlyph(Color.Gray, Color.Black, 205),
            new ColoredGlyph(Color.Gray, Color.Black, 188),
            new ColoredGlyph(Color.Gray, Color.Black, 187),
            new ColoredGlyph(Color.Gray, Color.Black, 185),
            new ColoredGlyph(Color.Gray, Color.Black, 205),
            new ColoredGlyph(Color.Gray, Color.Black, 200),
            new ColoredGlyph(Color.Gray, Color.Black, 201),
            new ColoredGlyph(Color.Gray, Color.Black, 204),
            new ColoredGlyph(Color.Gray, Color.Black, 205),
            new ColoredGlyph(Color.Gray, Color.Black, 202),
            new ColoredGlyph(Color.Gray, Color.Black, 203),
            new ColoredGlyph(Color.Gray, Color.Black, 206)
        };

        private static ColoredGlyph[] WallsVisible = new ColoredGlyph[16]
        {
            new ColoredGlyph(Color.LimeGreen, Color.Black, 9),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 186),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 186),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 186),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 205),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 188),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 187),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 185),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 205),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 200),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 201),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 204),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 205),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 202),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 203),
            new ColoredGlyph(Color.LimeGreen, Color.Black, 206)
        };

        public static ColoredGlyph GetWallGlyph(Map map, int x, int y, bool visible)
        {
            int mask = 0;
            mask += map.CellRevealedAndIsWall(x, y - 1) ? 1 : 0;
            mask += map.CellRevealedAndIsWall(x, y + 1) ? 2 : 0;
            mask += map.CellRevealedAndIsWall(x - 1, y) ? 4 : 0;
            mask += map.CellRevealedAndIsWall(x + 1, y) ? 8 : 0;

            return visible ? WallsVisible[mask] : Walls[mask];
        }

    }
}

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
        Floor = '.'
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
        public static ColoredGlyph FloorVisible = new ColoredGlyph(Color.LimeGreen, Color.Black, (char)TileType.Floor);

    }
}

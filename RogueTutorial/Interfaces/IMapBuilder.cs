using RogueTutorial.Map;
using SadRogue.Primitives;
using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Interfaces
{
    public interface IMapBuilder
    {
        string GetName();
        Map.Map GetMap();
        void Build(int width, int height, int depth);
        void SpawnEntities(World world);
        Point GetStartingPosition();
        List<Map.Map> GetSnapshotHistory();
        void TakeSnapshot();

        public static void ApplyRoomToMap(ref Map.Rectangle room, ref Map.Map map)
        {
            for (int i = room.X1 + 1; i <= room.X2; i++)
            {
                for (int j = room.Y1 + 1; j <= room.Y2; j++)
                {
                    map.SetMapCell(i, j, TileType.Floor);
                }
            }
        }

        public static void ApplyHorizontalTunnel(ref Map.Map map, int x1, int x2, int y)
        {
            for (int i = Math.Min(x1, x2); i <= Math.Max(x1, x2); i++)
            {
                map.SetMapCell(i, y, TileType.Floor);
            }
        }

        public static void ApplyVerticalTunnel(ref Map.Map map, int y1, int y2, int x)
        {
            for (int j = Math.Min(y1, y2); j <= Math.Max(y1, y2); j++)
            {
                map.SetMapCell(x, j, TileType.Floor);
            }
        }

    }
}

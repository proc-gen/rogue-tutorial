using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadRogue.Primitives;

namespace RogueTutorial.Map
{
    internal class MapGenerator
    {
        public static Map Section2_2Generator(int width, int height)
        {
            Map map = new Map(width, height);

            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    if(i == 0 || j == 0 || i == (width - 1) || j == (height - 1))
                    {
                        map.SetMapCell(i, j, TileType.Wall);
                    }
                    else
                    {
                        map.SetMapCell(i, j, TileType.Floor);
                    }
                }
            }

            map.Rooms.Add(new Rectangle(0, 0, width, height));

            Random random = new Random();
            for(int i = 0; i < 400; i++)
            {
                int x = random.Next(1,width-1);
                int y = random.Next(1,height-1);
                if(x != width/2 && y != height / 2)
                {
                    map.SetMapCell(x, y, TileType.Wall);
                }
            }

            return map;
        }

        public static Map RoomsAndCorridorsGenerator(int width, int height)
        {
            var map = new Map(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    map.SetMapCell(i, j, TileType.Wall);
                }
            }

            int maxRooms = 30, minSize = 6, maxSize = 10;

            Random random = new Random();

            for(int i = 0; i < maxRooms; i++)
            {
                int roomWidth = random.Next(minSize,maxSize);
                int roomHeight = random.Next(minSize,maxSize);
                int x = random.Next(1, width - roomWidth - 1) - 1;
                int y = random.Next(1, height - roomHeight - 1) - 1;

                Rectangle room = new Rectangle(x, y, roomWidth, roomHeight);
                bool canAdd = true;
                if(map.Rooms.Any() && map.Rooms.Exists(a => a.Intersect(ref room)))
                {
                    canAdd = false;
                }
                if (canAdd)
                {
                    applyRoomToMap(ref room, ref map);
                    if (map.Rooms.Any())
                    {
                        Point newCenter = room.Center();
                        Point oldCenter = map.Rooms.Last().Center();

                        if(random.Next(0,2) == 1)
                        {
                            applyHorizontalTunnel(ref map, oldCenter.X, newCenter.X, oldCenter.Y);
                            applyVerticalTunnel(ref map, oldCenter.Y, newCenter.Y, newCenter.X);
                        }
                        else
                        {
                            applyVerticalTunnel(ref map, oldCenter.Y, newCenter.Y, oldCenter.X);
                            applyHorizontalTunnel(ref map, oldCenter.X, newCenter.X, newCenter.Y);
                        }
                    }
                    map.Rooms.Add(room);
                }
            }



            return map;
        }

        private static void applyRoomToMap(ref Rectangle room, ref Map map)
        {
            for(int i = room.X1 + 1; i <= room.X2; i++)
            {
                for(int j = room.Y1 + 1; j <= room.Y2; j++)
                {
                    map.SetMapCell(i,j, TileType.Floor);
                }
            }
        }

        private static void applyHorizontalTunnel(ref Map map, int x1, int x2, int y)
        {
            for(int i = Math.Min(x1,x2); i <= Math.Max(x1,x2); i++)
            {
                map.SetMapCell(i,y, TileType.Floor);
            }
        }

        private static void applyVerticalTunnel(ref Map map, int y1, int y2, int x)
        {
            for (int j = Math.Min(y1, y2); j <= Math.Max(y1, y2); j++)
            {
                map.SetMapCell(x, j, TileType.Floor);
            }
        }
    }
}

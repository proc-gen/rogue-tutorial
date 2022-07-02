using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Map
{
    internal class MapGenerator
    {
        public static Map DefaultGenerator(int width, int height)
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

            var random = new Random();
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
    }
}

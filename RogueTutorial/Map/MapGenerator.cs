using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadRogue.Primitives;
using RogueTutorial.Interfaces;

namespace RogueTutorial.Map
{
    internal class MapGenerator
    {
        public static IMapBuilder BuildRandomMap(int width, int height, int depth)
        {
            IMapBuilder mapBuilder = null;
            Random random = new Random();
            switch (random.Next(2))
            {
                case 0:
                    mapBuilder = new SimpleMapBuilder();
                    break;
                default:
                    mapBuilder = new BspDungeonBuilder();
                    break;
            }
            
            mapBuilder.Build(width, height, depth);
            return mapBuilder;
        }
    }
}

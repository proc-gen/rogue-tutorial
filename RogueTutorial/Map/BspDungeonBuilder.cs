using RogueTutorial.Interfaces;
using SadRogue.Primitives;
using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Map
{
    public class BspDungeonBuilder : IMapBuilder
    {
        private Map _map;
        private Point _startingPosition = new Point(0, 0);
        private List<Rectangle> _rooms { get; set; }
        private List<Rectangle> _rectangles { get; set; }
        private List<Map> _snapshots { get; set; }

        public BspDungeonBuilder()
        {
            _rooms = new List<Rectangle>();
            _rectangles = new List<Rectangle>();
            _snapshots = new List<Map>();
        }

        public string GetName()
        {
            return "BSP Dungeon Builder";
        }

        public Map GetMap()
        {
            return _map;
        }
        public void Build(int width, int height, int depth)
        {
            _map = new Map(width, height, depth);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    _map.SetMapCell(i, j, TileType.Wall);
                }
            }

            Random random = new Random();

            buildRooms(random);
            buildCorridors(random);

            setStairsPosition();
            _startingPosition = _rooms.First().Center();
            TakeSnapshot();
        }

        private void buildRooms(Random random)
        {

            _rectangles.Add(new Rectangle(2, 2, _map.Width - 5, _map.Height - 5));
            addSubrectangles(_rectangles[0]);

            int numRooms = 0;
            while (numRooms < 240)
            {
                Rectangle rectangle = getRandomRectangle(random);
                Rectangle candidate = getRandomSubRectangle(rectangle, random);
                if (isPossible(candidate))
                {
                    IMapBuilder.ApplyRoomToMap(ref candidate, ref _map);
                    _rooms.Add(candidate);
                    addSubrectangles(candidate);
                    TakeSnapshot();
                }
                numRooms++;
            };

            _rooms = _rooms.OrderBy(a => a.X1).ToList();
        }

        private void addSubrectangles(Rectangle rectangle)
        {
            int width = Math.Abs(rectangle.X1 - rectangle.X2);
            int height = Math.Abs(rectangle.Y1 - rectangle.Y2);
            int halfWidth = Math.Max(width / 2, 1);
            int halfHeight = Math.Max(height / 2, 1);

            _rectangles.Add(new Rectangle(rectangle.X1, rectangle.Y1, halfWidth, halfHeight));
            _rectangles.Add(new Rectangle(rectangle.X1 + halfWidth, rectangle.Y1, halfWidth, halfHeight));
            _rectangles.Add(new Rectangle(rectangle.X1, rectangle.Y1 + halfHeight, halfWidth, halfHeight));
            _rectangles.Add(new Rectangle(rectangle.X1 + halfWidth, rectangle.Y1 + halfHeight, halfWidth, halfHeight));

        }

        private Rectangle getRandomRectangle(Random random)
        {
            return _rectangles.Count == 1 
                ? _rectangles[0]
                : _rectangles[random.Next(0, _rectangles.Count)];
        }

        private Rectangle getRandomSubRectangle(Rectangle rectangle, Random random)
        {
            int width = Math.Abs(rectangle.X1 - rectangle.X2);
            int height = Math.Abs(rectangle.Y1 - rectangle.Y2);

            int w = Math.Max(3, random.Next(Math.Min(width, 10))) + 1;
            int h = Math.Max(3, random.Next(Math.Min(height, 10))) + 1;
            return new Rectangle(rectangle.X1 + random.Next(6)
                                , rectangle.Y1 + random.Next(6), w, h);
        }

        private bool isPossible(Rectangle rectangle)
        {
            Rectangle expanded = new Rectangle(rectangle.X1 - 2
                                                , rectangle.Y1 - 2
                                                , rectangle.X2 - rectangle.X1 + 4
                                                , rectangle.Y2 - rectangle.Y1 + 4);
            bool canBuild = true;

            for(int y = expanded.Y1; y < expanded.Y2; y++)
            {
                for(int x = expanded.X1; x < expanded.X2; x++)
                {
                    if((x > (_map.Width - 2))
                        || (x < 1)
                        || (y > (_map.Height - 2))
                        || (y < 1)
                        || _map.GetMapCell(x,y) != TileType.Wall)
                    {
                        canBuild = false;
                    }
                }
            }

            return canBuild;
        }

        private void buildCorridors(Random random)
        {
            for(int i = 0; i < (_rooms.Count - 1); i++)
            {
                Rectangle room = _rooms[i];
                Rectangle nextRoom = _rooms[i+1];

                int startX = room.X1 + random.Next(Math.Abs(room.X2 - room.X1));
                int startY = room.Y1 + random.Next(Math.Abs(room.Y2 - room.Y1));
                int endX = nextRoom.X1 + random.Next(Math.Abs(nextRoom.X2 - nextRoom.X1));
                int endY = nextRoom.Y1 + random.Next(Math.Abs(nextRoom.Y2 - nextRoom.Y1));

                buildCorridor(startX, endX, startY, endY);
                TakeSnapshot();
            }
        }

        private void buildCorridor(int x1, int x2, int y1, int y2)
        {
            int x = x1;
            int y = y1;

            while(x != x2 || y != y2)
            {
                if (x < x2) 
                {
                    x++;
                }
                else if (x > x2) 
                {
                    x--;
                }
                else if (y < y2) 
                {
                    y++;
                }
                else if (y > y2) 
                {
                    y--;
                }

                _map.SetMapCell(x, y, TileType.Floor);
            }
        }

        private void setStairsPosition()
        {
            _map.SetMapCell(_rooms.Last().Center().X, _rooms.Last().Center().Y, TileType.DownStairs);
        }

        public void SpawnEntities(World world)
        {
            if (_map != null)
            {
                for (int i = 1; i < _rooms.Count; i++)
                {
                    Spawner.SpawnRoom(world, _rooms[i]);
                }
            }
        }

        public Point GetStartingPosition()
        {
            return _startingPosition;
        }

        public List<Map> GetSnapshotHistory()
        {
            return _snapshots;
        }

        public void TakeSnapshot()
        {
            if (RootScreen.SHOW_MAPGEN_VISUALIZER)
            {
                _snapshots.Add(_map.CopyForSnapshot());
            }
        }
    }
}

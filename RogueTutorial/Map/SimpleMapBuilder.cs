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
    public class SimpleMapBuilder : IMapBuilder
    {
        private Map _map;
        private Point _startingPosition = new Point(0, 0);
        private List<Rectangle> _rooms { get; set; }
        private List<Map> _snapshots { get; set; }

        public SimpleMapBuilder()
        {
            _rooms = new List<Rectangle>();
            _snapshots = new List<Map>();
        }

        public string GetName()
        {
            return "Simple Map Builder";
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

            int maxRooms = 30, minSize = 6, maxSize = 10;

            Random random = new Random();

            for (int i = 0; i < maxRooms; i++)
            {
                int roomWidth = random.Next(minSize, maxSize);
                int roomHeight = random.Next(minSize, maxSize);
                int x = random.Next(1, width - roomWidth - 1) - 1;
                int y = random.Next(1, height - roomHeight - 1) - 1;

                Rectangle room = new Rectangle(x, y, roomWidth, roomHeight);
                bool canAdd = true;
                if (_rooms.Any() && _rooms.Exists(a => a.Intersect(ref room)))
                {
                    canAdd = false;
                }
                if (canAdd)
                {
                    IMapBuilder.ApplyRoomToMap(ref room, ref _map);
                    TakeSnapshot();

                    if (_rooms.Any())
                    {
                        Point newCenter = room.Center();
                        Point oldCenter = _rooms.Last().Center();

                        if (random.Next(0, 2) == 1)
                        {
                            IMapBuilder.ApplyHorizontalTunnel(ref _map, oldCenter.X, newCenter.X, oldCenter.Y);
                            IMapBuilder.ApplyVerticalTunnel(ref _map, oldCenter.Y, newCenter.Y, newCenter.X);
                        }
                        else
                        {
                            IMapBuilder.ApplyVerticalTunnel(ref _map, oldCenter.Y, newCenter.Y, oldCenter.X);
                            IMapBuilder.ApplyHorizontalTunnel(ref _map, oldCenter.X, newCenter.X, newCenter.Y);
                        }
                    }
                    _rooms.Add(room);
                    TakeSnapshot();
                }
            }

            setStairsPosition();
            _startingPosition = _rooms.First().Center();
            TakeSnapshot();
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

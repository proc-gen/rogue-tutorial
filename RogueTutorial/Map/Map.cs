using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueSharp;

using Point = SadRogue.Primitives.Point;
using SimpleECS;

namespace RogueTutorial.Map
{
    internal class Map
    {
        private TileType[] _mapGrid;
        private bool[] _blocked;
        private Dictionary<Point, List<Entity>> _tileContent;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public List<Rectangle> Rooms { get; set; }

        private IMap map { get; set; }

        public Map(int width, int height)
        {
            Height = height;
            Width = width;
            _mapGrid = new TileType[width * height];
            _blocked = new bool[width * height];
            Rooms = new List<Rectangle>();
            map = new RogueSharp.Map(width, height);
            _tileContent = new Dictionary<Point, List<Entity>>();
        }

        public TileType GetMapCell(int x, int y)
        {
            return _mapGrid[y * Width + x];
        }

        public void SetMapCell(int x, int y, TileType tileType)
        {
            _mapGrid[y * Width + x] = tileType;
            _blocked[y * Width + x] = tileType == TileType.Wall;
            map.SetCellProperties(x, y, tileType == TileType.Floor, tileType == TileType.Floor, false);
        }

        public bool IsCellWalkable(int x, int y)
        {
            return !_blocked[y * Width + x];//map.IsWalkable(x, y);
        }

        public void SetCellWalkable(int x, int y, bool walkable)
        {
            ICell cell = map.GetCell(x, y);
            map.SetCellProperties(x, y, cell.IsTransparent, walkable);
            _blocked[y * Width + x] = !walkable;
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

        public Path FindPath(Point start, Point end)
        {
            ICell oldStartCell = map.GetCell(start.X, start.Y);
            ICell oldEndCell = map.GetCell(end.X, end.Y);
            
            //Must make cells walkable prior to pathfinding
            //map.SetCellProperties(oldStartCell.X, oldStartCell.Y, true, true);
            //map.SetCellProperties(oldEndCell.X, oldEndCell.Y, true, true);

            PathFinder pathFinder = new PathFinder(map, 1.45);
            Path path = pathFinder.TryFindShortestPath(oldStartCell, oldEndCell);

            //map.SetCellProperties(oldStartCell.X, oldStartCell.Y, oldStartCell.IsTransparent, oldStartCell.IsWalkable);
            //map.SetCellProperties(oldEndCell.X, oldEndCell.Y, oldEndCell.IsTransparent, oldEndCell.IsWalkable);

            return path;
        }

        public void ResetTileContent()
        {
            _tileContent.Clear();
        }

        public void AddCellEntity(Entity entity, Point point)
        {
            if (_tileContent.ContainsKey(point))
            {
                _tileContent[point].Add(entity);
            }
            else
            {
                _tileContent[point] = new List<Entity>() { entity };
            }
        }

        public List<Entity> GetCellEntities(Point point)
        {
            return _tileContent.ContainsKey(point) ? _tileContent[point] : new List<Entity>();
        }
    }
}

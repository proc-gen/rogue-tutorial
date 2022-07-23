using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueSharp;

using Point = SadRogue.Primitives.Point;
using SimpleECS;
using RogueTutorial.Interfaces;
using RogueTutorial.Utils;

namespace RogueTutorial.Map
{
    public class Map : ISaveable
    {
        private TileType[] _mapGrid;
        private bool[] _blocked;
        private bool[] _explored;
        private Dictionary<Point, List<Entity>> _tileContent;
        private Dictionary<Point, bool> _bloodyTiles;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Depth { get; private set; }
        public List<Rectangle> Rooms { get; set; }

        private IMap map { get; set; }

        public Map(int width, int height, int depth = 1)
        {
            Height = height;
            Width = width;
            Depth = depth;

            _mapGrid = new TileType[width * height];
            _blocked = new bool[width * height];
            _explored = new bool[width * height];

            Rooms = new List<Rectangle>();
            map = new RogueSharp.Map(width, height);
            _tileContent = new Dictionary<Point, List<Entity>>();
            _bloodyTiles = new Dictionary<Point, bool>();
        }

        public TileType GetMapCell(int x, int y)
        {
            return _mapGrid[y * Width + x];
        }

        public TileType GetMapCellForRender(int x, int y)
        {
            if (_mapGrid[y * Width + x] == TileType.Floor && _bloodyTiles.ContainsKey(new Point(x,y)))
            {
                return TileType.BloodyFloor;
            }
            return _mapGrid[y * Width + x];
        }

        public bool CellRevealedAndIsWall(int x, int y)
        {
            bool retVal = false;
            if(x >=0 && y >= 0 && x < Width && y < Height)
            {
                retVal = GetMapCell(x, y) == TileType.Wall && IsMapCellExplored(x, y);
            }
            return retVal;
        }

        public void SetMapCell(int x, int y, TileType tileType)
        {
            _mapGrid[y * Width + x] = tileType;
            _blocked[y * Width + x] = tileType == TileType.Wall;
            map.SetCellProperties(x, y, tileType != TileType.Wall, tileType != TileType.Wall, false);
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
                    _explored[cell.Y * Width + cell.X] = true;
                }
            }

            return cells.Select(cell => new Point(cell.X, cell.Y)).ToList();
        }

        private void updateCellVisibility(int x, int y, bool visibility)
        {
            TileType tileType = GetMapCell(x, y);
            map.SetCellProperties(x, y, tileType != TileType.Wall, tileType != TileType.Wall, visibility);
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
            map.SetCellProperties(oldStartCell.X, oldStartCell.Y, true, true);
            map.SetCellProperties(oldEndCell.X, oldEndCell.Y, true, true);

            PathFinder pathFinder = new PathFinder(map, 1.45);
            Path path = pathFinder.TryFindShortestPath(map.GetCell(start.X, start.Y), map.GetCell(end.X, end.Y));

            map.SetCellProperties(oldStartCell.X, oldStartCell.Y, oldStartCell.IsTransparent, oldStartCell.IsWalkable);
            map.SetCellProperties(oldEndCell.X, oldEndCell.Y, oldEndCell.IsTransparent, oldEndCell.IsWalkable);

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

        public void SetBloodyCell(Point point)
        {
            _bloodyTiles[point] = true;
        }

        public StringBuilder Save(StringBuilder sb, int index)
        {
            sb.AppendLine("Map:" + index.ToString());
            sb.AppendLine("Width:" + Width.ToString());
            sb.AppendLine("Height:" + Height.ToString());
            sb.AppendLine("Depth:" + Depth.ToString());
            for (int i = 0; i < Rooms.Count; i++)
            {
                sb = Rooms[i].Save(sb, i);
            }
            for(int i = 0; i < Width; i++)
            {
                for(int j = 0; j < Height; j++)
                {
                    sb.Append("Point(" + i + "," + j + "):");
                    sb.Append(_blocked[j * Width + i].ToString() + ",");
                    sb.Append(_explored[j * Width + i].ToString() + ",");
                    sb.Append(_mapGrid[j * Width + i].ToString() + ",");
                    sb.Append(_bloodyTiles.ContainsKey(new Point(i, j)));
                    sb.AppendLine();
                }
            }
            return sb;
        }

        public void Load(List<LineData> data, int index)
        {
            index = loadRooms(data, index);
            index = populateCells(data, index);
        }

        private int loadRooms(List<LineData> data, int index)
        {
            while (data[index].FieldName == "Rectangle")
            {
                Rectangle room = new Rectangle(0, 0, 0, 0);
                room.Load(data, index);
                Rooms.Add(room);
                index += 5;
            }

            return index;
        }

        private int populateCells(List<LineData> data, int index)
        {
            do
            {
                Point position = SaveGameManager.GetPointFromFieldValue(data[index].FieldName.Replace("Point(", "").Replace(")", ""));
                string[] cellData = data[index].FieldValue.Split(",");
                _blocked[position.Y * Width + position.X] = bool.Parse(cellData[0]);
                _explored[position.Y * Width + position.X] = bool.Parse(cellData[1]);
                _mapGrid[position.Y * Width + position.X] = TileTypeExtensions.GetTileTypeFromString(cellData[2]);
                if (bool.Parse(cellData[3]))
                {
                    _bloodyTiles[position] = true;
                }
                map.SetCellProperties(position.X, position.Y, !_blocked[position.Y * Width + position.X], !_blocked[position.Y * Width + position.X], _explored[position.Y * Width + position.X]);
                index++;
            }while(index < data.Count);
            return index;
        }
    }
}

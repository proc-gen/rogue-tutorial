using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueTutorial.Interfaces;
using RogueTutorial.Utils;
using SimpleECS;

namespace RogueTutorial.Components
{
    internal class Position : ISaveableComponent
    {
        private Point _point;
        public Point Point 
        { 
            get 
            { 
                return _point; 
            } 
            
            set
            { 
                PreviousPoint = _point;
                Dirty = true; 
                _point = value; 
            } 
        }
        public Point PreviousPoint { get; private set; }
        public bool Dirty { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            _point = SaveGameManager.GetPointFromFieldValue(componentData[0].FieldValue);
            PreviousPoint = SaveGameManager.GetPointFromFieldValue(componentData[1].FieldValue);
            Dirty = false;
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:Position");
            sb.AppendLine("Point:" + Point.X.ToString() + "," + Point.Y.ToString());
            sb.AppendLine("PreviousPoint:" + PreviousPoint.X.ToString() + "," + PreviousPoint.Y.ToString());
            return sb;
        }
    }
}

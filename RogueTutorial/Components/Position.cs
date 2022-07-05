using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Components
{
    internal class Position
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
    }
}

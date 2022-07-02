using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.Components;
using SadRogue.Primitives;
using SimpleECS;

namespace RogueTutorial.Systems
{
    internal class LeftMoverSystem : System
    {
        public LeftMoverSystem(Query query) : base(query)
        {
        }

        public override void Run(TimeSpan delta)
        {
            query.Foreach((ref Position position) =>
            {
                Point newPoint = position.Point.Add(Direction.Left);
                if (newPoint.X < 0)
                {
                    newPoint = new Point(newPoint.X + 80, newPoint.Y);
                }
                position.Point = newPoint;
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadConsole;
using SadRogue.Primitives;

namespace RogueTutorial.UI.Extensions
{
    public static class ConsoleDrawExtensions
    {
        public static void DrawRLTKStyleBox(this ICellSurface screen, int x, int y, int width, int height, Color foreground, Color background)
        {
            //Horizontal Lines
            screen.DrawLine(new Point(x + 1, y), new Point(x + width - 1, y), 196, foreground, background);
            screen.DrawLine(new Point(x + 1, y + height), new Point(x + width - 1, y + height), 196, foreground, background);
            
            //Vertical Lines
            screen.DrawLine(new Point(x, y + 1), new Point(x, y + height - 1), 179, foreground, background);
            screen.DrawLine(new Point(x + width, y + 1), new Point(x + width, y + height - 1), 179, foreground, background);

            //Corners
            screen.SetGlyph(x, y, 218, foreground, background);
            screen.SetGlyph(x + width, y + height, 217, foreground, background);
            screen.SetGlyph(x, y + height, 192, foreground, background);
            screen.SetGlyph(x + width, y, 191, foreground, background);
        }

        public static void DrawRLTKHorizontalBar(this ICellSurface screen, int x, int y, int maxWidth, int currentValue, int maxValue, Color foreground, Color background)
        {
            int width = (maxWidth - 1) * currentValue / maxValue;
            screen.DrawLine(new Point(x, y), new Point(x + width, y), 178, foreground, background);
        }
    }
}

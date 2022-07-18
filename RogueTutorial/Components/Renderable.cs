using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueTutorial.Interfaces;
using RogueTutorial.Utils;
using SimpleECS;
using SadConsole;

namespace RogueTutorial.Components
{
    internal class Renderable : ISaveableComponent
    {
        public ColoredGlyph Glyph { get; set; }
        public int RenderOrder { get; set; }

        public void Load(List<LineData> componentData, Entity[] entities)
        {
            Glyph = new ColoredGlyph(SaveGameManager.GetColorFromFieldValue(componentData[1].FieldValue)
                                      , SaveGameManager.GetColorFromFieldValue(componentData[2].FieldValue)
                                      , componentData[0].FieldValue[0]);
            RenderOrder = int.Parse(componentData[3].FieldValue);
        }

        public StringBuilder Save(StringBuilder sb, Entity[] entities)
        {
            sb.AppendLine("Component:Renderable");
            sb.AppendLine("Glyph:" + Glyph.GlyphCharacter);
            sb.AppendLine("GlyphColor:" + Glyph.Foreground.R + "," + Glyph.Foreground.G + "," + Glyph.Foreground.B + "," + Glyph.Foreground.A);
            sb.AppendLine("GlyphBackColor:" + Glyph.Background.R + "," + Glyph.Background.G + "," + Glyph.Background.B + "," + Glyph.Background.A);
            sb.AppendLine("RenderOrder:" + RenderOrder.ToString());
            return sb;
        }
    }
}

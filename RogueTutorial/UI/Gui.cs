using SadConsole;
using SadRogue.Primitives;
using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueTutorial.UI.Extensions;
using RogueTutorial.Components;
using RogueTutorial.Helpers;

namespace RogueTutorial.UI
{
    internal class Gui
    {
        private World _world;
        private Entity _player;
        private Query _tooltipQuery;
        public Gui(World world, Query playerQuery)
        {
            _world = world;
            _player = playerQuery.GetEntities()[0];
            _tooltipQuery = world.CreateQuery()
                                .Has<Position>()
                                .Has<Name>();
        }

        public void Render(ICellSurface screen, Point? mousePosition)
        {
            screen.DrawRLTKStyleBox(0, 39, 79, 10, Color.White, Color.Black);
            drawPlayerStats(screen);
            drawGameLog(screen);
            drawTooltips(screen, mousePosition);
        }

        private void drawPlayerStats(ICellSurface screen)
        {
            CombatStats playerStats = _player.Get<CombatStats>();
            screen.Print(12, 39, " HP: " + playerStats.Hp + " / " + playerStats.MaxHp + " ", Color.Yellow, Color.Black);
            screen.DrawRLTKHorizontalBar(28, 39, 51, playerStats.Hp, playerStats.MaxHp, Color.Red, Color.Black);
        }

        private void drawGameLog(ICellSurface screen)
        {
            GameLog log = _world.GetData<GameLog>();
            int y = 40;
            for(int i = 1; i <= Math.Min(9, log.Entries.Count); i++)
            {
                screen.Print(2, y, log.Entries[log.Entries.Count - i]);
                y++;
            }
        }

        private void drawTooltips(ICellSurface screen, Point? mousePosition)
        {
            if (mousePosition.HasValue)
            {
                screen.SetGlyph(mousePosition.Value.X, mousePosition.Value.Y, 176, Color.Magenta);

                if(_tooltipQuery.GetEntities().Any(a => a.Get<Position>().Point == mousePosition.Value))
                {
                    Entity entity = _tooltipQuery.GetEntities().First(a => a.Get<Position>().Point == mousePosition.Value);
                    string toolTip = entity.Get<Name>().EntityName;
                    
                    if((toolTip.Length + 4 + mousePosition.Value.X) > screen.Width)
                    {
                        screen.Print(mousePosition.Value.X - toolTip.Length - 3, mousePosition.Value.Y, toolTip + " ->", Color.White, Color.DarkGray);
                    }
                    else
                    {
                        screen.Print(mousePosition.Value.X + 1, mousePosition.Value.Y, "<- " + toolTip, Color.White, Color.DarkGray);
                    }
                }
            }
        }
    }
}

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
using SadConsole.Input;

namespace RogueTutorial.UI
{
    internal class Gui
    {
        private World _world;
        private Entity _player;
        private Query _tooltipQuery;
        private Query _inventoryQuery;
        public Gui(World world, Query playerQuery)
        {
            _world = world;
            _player = playerQuery.GetEntities()[0];
            _tooltipQuery = world.CreateQuery()
                                .Has<Position>()
                                .Has<Name>();
            _inventoryQuery = world.CreateQuery()
                                .Has<InBackpack>();
        }

        public void Render(ICellSurface screen, Point? mousePosition)
        {
            screen.DrawRLTKStyleBox(0, 39, 79, 10, Color.White, Color.Black);
            drawPlayerStats(screen);
            drawGameLog(screen);

            if (_world.GetData<RunState>() == RunState.ShowInventory)
            {
                drawInventory(screen);
            }
            else
            {
                drawTooltips(screen, mousePosition);
            }
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

                if(_tooltipQuery.GetEntities().Any(a => _player.Get<Viewshed>().VisibleTiles.Any(b => b == a.Get<Position>().Point) && a.Get<Position>().Point == mousePosition.Value))
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

        private void drawInventory(ICellSurface screen)
        {
            IEnumerable<Entity> inventoryItems = getInventoryItems(_player);

            int y = 25 - (inventoryItems.Count() / 2);
            screen.DrawRLTKStyleBox(15, y - 2, 31, inventoryItems.Count() + 3, Color.White, Color.Black);
            screen.Print(18, y - 2, "Inventory", Color.Yellow, Color.Black);
            screen.Print(18, y + inventoryItems.Count() + 1, "ESCAPE to cancel", Color.Yellow, Color.Black);

            int i = 0;
            foreach(Entity item in inventoryItems)
            {
                char c = (char)(64 + (i + 1));
                screen.Print(17, y + i, "(", Color.White, Color.Black);
                screen.Print(18, y + i, c.ToString(), Color.Yellow, Color.Black);
                screen.Print(19, y + i, ")", Color.White, Color.Black);
                screen.Print(21, y + i, item.Get<Name>().EntityName, Color.White, Color.Black);
                i++;
            }
        }

        private IEnumerable<Entity> getInventoryItems(Entity owner)
        {
            IEnumerable<Entity> inventoryItems = _inventoryQuery.GetEntities().Where(a => a.Get<InBackpack>().Owner == owner).OrderBy(a => a.index).ThenBy(a => a.version);
            return inventoryItems;
        }

        public void ProcessKeyboardInventory(Keyboard keyboard, ICellSurface screen)
        {
            IEnumerable<Entity> inventoryItems = getInventoryItems(_player);

            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                screen.IsDirty = true;
                _world.SetData(RunState.AwaitingInput);
            }

            Keys keyToCheck = Keys.A;
            foreach(Entity item in inventoryItems)
            {
                if (keyboard.IsKeyPressed(keyToCheck))
                {
                    _player.Set(new WantsToDrinkPotion() { Potion = item });
                    _world.SetData(RunState.PlayerTurn);
                    screen.IsDirty = true;
                    return;
                }
                keyToCheck++;
            }
        }
    }
}

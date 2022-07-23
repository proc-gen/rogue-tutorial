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
    internal class GameGui
    {
        private World _world;
        private Entity _player { get { return _playerQuery.GetEntities()[0]; } }
        private Query _playerQuery;
        private Query _tooltipQuery;
        private Query _inventoryQuery;
        private Query _itemForTargetQuery;
        private Query _equippedItemsQuery;
        public GameGui(World world, Query playerQuery)
        {
            _world = world;
            _playerQuery = playerQuery;
            _tooltipQuery = world.CreateQuery()
                                .Has<Position>()
                                .Has<Name>();
            _inventoryQuery = world.CreateQuery()
                                .Has<InBackpack>();
            _itemForTargetQuery = world.CreateQuery()
                                   .Has<UseForTargeting>();
            _equippedItemsQuery = world.CreateQuery()
                                .Has<Equipped>();
        }

        public void Render(ICellSurface screen, Point? mousePosition)
        {
            screen.DrawRLTKStyleBox(0, 39, 79, 10, Color.White, Color.Black);
            drawPlayerStats(screen);
            drawGameLog(screen);

            switch (_world.GetData<RunState>())
            {
                case RunState.ShowInventory:
                    drawInventory(screen, "Inventory");
                    break;
                case RunState.ShowDropItem:
                    drawInventory(screen, "Drop Which Item?");
                    break;
                case RunState.ShowTargeting:
                    drawTargetingSystem(screen, mousePosition);
                    break;
                case RunState.ShowRemoveItem:
                    drawEquippedItems(screen);
                    break;
                default:
                    drawTooltips(screen, mousePosition);
                    break;
            }
        }

        private void drawPlayerStats(ICellSurface screen)
        {
            CombatStats playerStats = _player.Get<CombatStats>();
            screen.Print(2, 39, "Depth: " + _world.GetData<Map.Map>().Depth, Color.Yellow, Color.Black);
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

        private void drawTargetingSystem(ICellSurface screen, Point? mousePosition)
        {
            Entity forTargeting = _itemForTargetQuery.GetEntities()[0].Get<UseForTargeting>().Item;
            Ranged targetRange = forTargeting.Get<Ranged>();
            

            screen.Print(5, 0, "Select Target for " + forTargeting.Get<Name>().EntityName, Color.Yellow, Color.Black);

            if (mousePosition.HasValue)
            {
                if (_player.Get<Viewshed>().VisibleTiles.Any(a => a == mousePosition.Value)
                        && Point.EuclideanDistanceMagnitude(_player.Get<Position>().Point, mousePosition.Value) <= (targetRange.Range * targetRange.Range))
                {
                    AreaOfEffect aoe = null;
                    if(forTargeting.TryGet(out aoe))
                    {
                        List<Point> targetCells = new List<Point>();
                        targetCells.AddRange(_world.GetData<Map.Map>().ComputeFOV(mousePosition.Value.X, mousePosition.Value.Y, aoe.Radius, false, false));
                        foreach(Point targetCell in targetCells)
                        {
                            screen.SetGlyph(targetCell.X, targetCell.Y, 176, Color.Blue);
                        }
                    }
                    screen.SetGlyph(mousePosition.Value.X, mousePosition.Value.Y, 176, Color.Cyan);
                }
                else
                {
                    screen.SetGlyph(mousePosition.Value.X, mousePosition.Value.Y, 176, Color.Red);
                }
            }
        }

        private void drawInventory(ICellSurface screen, string title)
        {
            IEnumerable<Entity> inventoryItems = getInventoryItems(_player);

            int y = 25 - (inventoryItems.Count() / 2);
            screen.DrawRLTKStyleBox(15, y - 2, 31, inventoryItems.Count() + 3, Color.White, Color.Black);
            screen.Print(18, y - 2, title, Color.Yellow, Color.Black);
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

        private void drawEquippedItems(ICellSurface screen)
        {
            IEnumerable<Entity> equippedItems = getEquippedItems(_player);

            int y = 25 - (equippedItems.Count() / 2);
            screen.DrawRLTKStyleBox(15, y - 2, 31, equippedItems.Count() + 3, Color.White, Color.Black);
            screen.Print(18, y - 2, "Remove Which Item?", Color.Yellow, Color.Black);
            screen.Print(18, y + equippedItems.Count() + 1, "ESCAPE to cancel", Color.Yellow, Color.Black);

            int i = 0;
            foreach (Entity item in equippedItems)
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

        private IEnumerable<Entity> getEquippedItems(Entity owner)
        {
            IEnumerable<Entity> inventoryItems = _equippedItemsQuery.GetEntities().Where(a => a.Get<Equipped>().Owner == owner).OrderBy(a => a.index).ThenBy(a => a.version);
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
                    _player.Set(new WantsToUseItem() { Item = item });
                    _world.SetData(RunState.PlayerTurn);
                    screen.IsDirty = true;
                    return;
                }
                keyToCheck++;
            }
        }

        public void ProcessKeyboardDropItem(Keyboard keyboard, ICellSurface screen)
        {
            IEnumerable<Entity> inventoryItems = getInventoryItems(_player);

            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                screen.IsDirty = true;
                _world.SetData(RunState.AwaitingInput);
            }

            Keys keyToCheck = Keys.A;
            foreach (Entity item in inventoryItems)
            {
                if (keyboard.IsKeyPressed(keyToCheck))
                {
                    _player.Set(new WantsToDropItem() { Item = item });
                    _world.SetData(RunState.PlayerTurn);
                    screen.IsDirty = true;
                    return;
                }
                keyToCheck++;
            }
        }

        public void ProcessKeyboardRemoveItem(Keyboard keyboard, ICellSurface screen)
        {
            IEnumerable<Entity> equippedItems = getEquippedItems(_player);

            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                screen.IsDirty = true;
                _world.SetData(RunState.AwaitingInput);
            }

            Keys keyToCheck = Keys.A;
            foreach (Entity item in equippedItems)
            {
                if (keyboard.IsKeyPressed(keyToCheck))
                {
                    _player.Set(new WantsToRemoveItem() { Item = item });
                    _world.SetData(RunState.PlayerTurn);
                    screen.IsDirty = true;
                    return;
                }
                keyToCheck++;
            }
        }

        public void ProcessKeyboardTargetingSystem(Keyboard keyboard, ICellSurface screen)
        {
            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                _player.Remove<WantsToUseItem>();
                screen.IsDirty = true;
                _world.SetData(RunState.AwaitingInput);
            }
        }
    
        public void ProcessMouseTargetingSystem(MouseScreenObjectState state)
        {
            if (state.Mouse.LeftClicked)
            {
                Entity forTargeting = _itemForTargetQuery.GetEntities()[0].Get<UseForTargeting>().Item;
                Ranged targetRange = forTargeting.Get<Ranged>();
                
                if (_player.Get<Viewshed>().VisibleTiles.Any(a => a == state.CellPosition)
                            && Point.EuclideanDistanceMagnitude(_player.Get<Position>().Point, state.CellPosition) <= (targetRange.Range * targetRange.Range))
                {
                    WantsToUseItem wantsUse = _player.Get<WantsToUseItem>();
                    wantsUse.Target = state.CellPosition;
                    _world.SetData(RunState.PlayerTurn);
                }
            }
        }
    }

}

using SimpleECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;

using RogueTutorial.Helpers;
using RogueTutorial.UI.Extensions;

namespace RogueTutorial.UI
{
    internal class MenuGui
    {
        private World _world;
        private MenuState _menuState;

        public MenuGui(World world)
        {
            _world = world;
            _menuState = MenuState.NewGame;
        }

        public void Render(ICellSurface screen, Point? mousePosition)
        {
            printTitle(screen);

            int optionPosition = 24;

            printMenuOption(screen, screen.Width / 2 - 4, optionPosition, "New Game", MenuState.NewGame);
            optionPosition++;

            if (showContinue())
            {
                printMenuOption(screen, screen.Width / 2 - 6, optionPosition, "Continue Game", MenuState.ContinueGame);
                optionPosition++;
            }

            printMenuOption(screen, screen.Width / 2 - 4, optionPosition, "Load Game", MenuState.LoadGame);
            optionPosition++;

            printMenuOption(screen, screen.Width / 2 - 4, optionPosition, "Quit Game", MenuState.Quit);
            optionPosition++;

        }

        private void printTitle(ICellSurface screen)
        {
            screen.DrawRLTKStyleBox(screen.Width / 2 - 12, 13, 27, 4, Color.White, Color.Black);
            screen.Print(screen.Width / 2 - 10, 15, "Rusty Roguelike Tutorial", Color.Yellow, Color.Black);
        }

        private void printMenuOption(ICellSurface screen, int x, int y, string text, MenuState option)
        {
            screen.Print(x, y, text, option == _menuState ? Color.Magenta : Color.White, Color.Black);
        }

        private bool showContinue()
        {
            return _world.EntityCount > 0;
        }

        public void ProcessKeyboard(Keyboard keyboard, ICellSurface screen)
        {
            if (keyboard.IsKeyPressed(Keys.Up) || keyboard.IsKeyPressed(Keys.NumPad8) || keyboard.IsKeyPressed(Keys.K))
            {
                screen.IsDirty = true;
                switch (_menuState)
                {
                    case MenuState.ContinueGame:
                        _menuState = MenuState.NewGame;
                        break;
                    case MenuState.LoadGame:
                        _menuState = showContinue() ? MenuState.ContinueGame : MenuState.NewGame;
                        break;
                    case MenuState.Quit:
                        _menuState = MenuState.LoadGame;
                        break;
                }
            }
            if (keyboard.IsKeyPressed(Keys.Down) || keyboard.IsKeyPressed(Keys.NumPad2) || keyboard.IsKeyPressed(Keys.J))
            {
                screen.IsDirty = true;
                switch (_menuState)
                {
                    case MenuState.NewGame:
                        _menuState = showContinue() ? MenuState.ContinueGame : MenuState.LoadGame;
                        break;
                    case MenuState.ContinueGame:
                        _menuState = MenuState.LoadGame;
                        break;
                    case MenuState.LoadGame:
                        _menuState = MenuState.Quit;
                        break;
                }
            }

            if (keyboard.IsKeyPressed(Keys.Enter))
            {
                screen.IsDirty = true;
                switch (_menuState)
                {
                    case MenuState.NewGame:
                        _world.SetData(RunState.PreRun);
                        break;
                    case MenuState.ContinueGame:
                        _world.SetData(RunState.AwaitingInput);
                        break;
                    case MenuState.LoadGame:
                        _world.SetData(RunState.PreRun);
                        break;
                    case MenuState.Quit:
                        Game.Instance.MonoGameInstance.Exit();
                        break;
                }
            }
        }

        public void ProcessMouse(MouseScreenObjectState state)
        {

        }
    }
}

using SadConsole;
using SadRogue.Primitives;
using System;
using Console = SadConsole.Console;

namespace RogueTutorial
{
    internal class Program
    {
        const int SCREEN_WIDTH = 80;
        const int SCREEN_HEIGHT = 25;
        private static void Main(string[] args)
        {
            Game.Create(SCREEN_WIDTH, SCREEN_HEIGHT);
            Game.Instance.OnStart = Init;
            Game.Instance.Run();
            Game.Instance.Dispose();
        }

        private static void Init()
        {
            Game.Instance.Screen = new RootScreen(SCREEN_WIDTH, SCREEN_HEIGHT);
            Game.Instance.Screen.IsFocused = true;
            Game.Instance.MonoGameInstance.Window.Title = "Rogue Tutorial";

            Game.Instance.DestroyDefaultStartingConsole();
        }
    }
}
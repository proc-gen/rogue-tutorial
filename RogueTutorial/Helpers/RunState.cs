using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueTutorial.Helpers
{
    public enum RunState
    {
        AwaitingInput,
        PreRun,
        PlayerTurn,
        MonsterTurn,
        ShowInventory,
        ShowDropItem,
        ShowTargeting,
        MainMenu,
        SaveGame,
        LoadGame,
        NextLevel,
        PlayerDeath,
    }
}

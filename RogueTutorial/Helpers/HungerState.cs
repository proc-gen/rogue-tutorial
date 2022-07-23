public enum HungerState
{
    WellFed = 3,
    Normal = 2,
    Hungry = 1, 
    Starving = 0
}

public static class HungerStateExtensions
{
    public static HungerState getHungerStateFromString(string slot)
    {
        switch (slot)
        {
            case "WellFed":
                return HungerState.WellFed;
                break;
            case "Normal":
                return HungerState.Normal;
                break;
            case "Hungry":
                return HungerState.Hungry;
                break;
            default:
                return HungerState.Starving;
                break;
        }
    }
}
using Celeste.Mod.SpeedrunTool.RoomTimer;

namespace Celeste.Mod.WonderMods.Streaks;
public static class StreakManager
{
    private static bool ManuallyResetFlag { get; set; } = false;
    private static bool ShouldSkipIncrement { get; set; } = false;
    private static bool ShouldResetCount { get; set; } = false;
    private static bool TimerStarted { get; set; } = false;

    public static void OnSaveState()
    {
        StreakCounter.ResetCount(false);
        StreakCounter.ResetBest(false);
    }

    public static void OnLoadState()
    {
        if (StreakCounter.StreakCounterType.Off == WonderModsModuleSettings.Instance.EnableStreaks) {
            StreakCounter.ResetCount(false);
            StreakCounter.ResetBest(false);
            return;
        }

        if (ShouldResetCount)
        {
            ShouldResetCount = false;
            StreakCounter.ResetCount(true);
        }
        else if (ShouldSkipIncrement)
        {
            ShouldSkipIncrement = false;
        }
        else if (TimerStarted)
        {
            StreakCounter.IncrementCount();
        }
    }

    public static void OnClearState()
    {
        StreakCounter.ResetCount(false);
        StreakCounter.ResetBest(false);
    }

    public static void OnBeforeSaveState()
    {
        StreakCounter.ResetCount(false);
    }

    public static void OnBeforeLoadState()
    {
        TimerStarted = RoomTimerManager.GetRoomTime() != 0;
        if (TimerStarted && !RoomTimerManager.TimerIsCompleted())
        {
            ShouldResetCount = WonderModsModuleSettings.Instance.EnableStreaks == StreakCounter.StreakCounterType.Auto;
        }
        if (ManuallyResetFlag)
        {
            if (!RoomTimerManager.TimerIsCompleted())
            {
                ShouldSkipIncrement = true;
            }
            ManuallyResetFlag = false;
        }
    }

    public static void ResetHotkey() {
        StreakCounter.ResetCount(true);
    }

    public static void IncrementHotkey() {
        StreakCounter.IncrementCount();
    }
}

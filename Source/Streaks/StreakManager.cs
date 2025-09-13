using System;
using System.Collections.Generic;
using Celeste.Mod.WonderMods.Integration;

namespace Celeste.Mod.WonderMods.Streaks;

public static class StreakManager
{
    private static bool ManuallyResetFlag { get; set; } = false;
    private static bool ShouldSkipIncrement { get; set; } = false;
    private static bool ShouldResetCount { get; set; } = false;
    private static bool TimerStarted { get; set; } = false;

    public static void OnSaveState(Dictionary<Type, Dictionary<string, object>> dictionary, Level level)
    {			
        WonderModsModule.WonderLog("Save state hook");
        StreakCounter.ResetCount(false);
        StreakCounter.ResetBest(false);
    }

    public static void OnLoadState(Dictionary<Type, Dictionary<string, object>> dictionary, Level level)
    {
        WonderModsModule.WonderLog("Load state hook");
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
        WonderModsModule.WonderLog("Clear state hook");
        StreakCounter.ResetCount(false);
        StreakCounter.ResetBest(false);
    }

    public static void OnBeforeSaveState(Level level)
    {
        WonderModsModule.WonderLog("Before save state hook");
        StreakCounter.ResetCount(false);
    }

    public static void OnBeforeLoadState(Level level)
    {
        WonderModsModule.WonderLog("Before load state hook");
        TimerStarted = RoomTimerIntegration.GetRoomTime() != 0;
        if (TimerStarted && !RoomTimerIntegration.RoomTimerIsCompleted())
        {
            ShouldResetCount = WonderModsModuleSettings.Instance.EnableStreaks == StreakCounter.StreakCounterType.Auto;
        }
        if (ManuallyResetFlag)
        {
            if (!RoomTimerIntegration.RoomTimerIsCompleted())
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

    public static void DecrementHotkey() {
        StreakCounter.DecrementCount();
    }
}

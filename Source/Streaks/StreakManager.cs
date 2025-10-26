using System;
using System.Collections.Generic;
using Celeste.Mod.WonderMods.Integration;
using Monocle;

namespace Celeste.Mod.WonderMods.Streaks;

public static class StreakManager
{
    private static bool ManuallyResetFlag { get; set; } = false;
    private static bool ShouldSkipIncrement { get; set; } = false;
    private static bool ShouldResetCount { get; set; } = false;
    private static bool TimerStarted { get; set; } = false;
    public static long LastRoomTime { get; set; } = 0;

    public static void Update()
    {
        if (RoomTimerIntegration.RoomTimerIsCompleted() && LastRoomTime == 0)
        {
            LastRoomTime = RoomTimerIntegration.GetRoomTime() - 170000; 
            WonderModsModule.WonderLog($"Update setting last room time {LastRoomTime}");
        }
    }

    public static void OnSaveState(Dictionary<Type, Dictionary<string, object>> dictionary, Level level)
    {			
        LastRoomTime = 0;
        StreakCounter.Reset(false);
    }

    public static void OnLoadState(Dictionary<Type, Dictionary<string, object>> dictionary, Level level)
    {
        if (StreakCounter.StreakCounterType.OFF == WonderModsModuleSettings.Instance.EnableStreaks)
        {
            StreakCounter.Reset(false);
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
            LastRoomTime = 0;
        }
    }

    public static void OnClearState()
    {
        StreakCounter.Reset(false);
    }

    public static void OnBeforeSaveState(Level level)
    {
        StreakCounter.ResetCount(false);
    }

    public static void OnBeforeLoadState(Level level)
    {
        TimerStarted = RoomTimerIntegration.GetRoomTime() != 0;
        if (TimerStarted && !RoomTimerIntegration.RoomTimerIsCompleted())
        {
            ShouldResetCount = WonderModsModuleSettings.Instance.EnableStreaks == StreakCounter.StreakCounterType.AUTO;
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

    public static void UndoHotkey() {
        StreakCounter.UndoCount();
    }

    public static void ExportHotkey()
    {
        StreakCounter.ExportStreakTimes();
    }
}

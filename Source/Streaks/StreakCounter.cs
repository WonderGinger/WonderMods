using Celeste.Mod.SpeedrunTool.Message;
using Celeste.Mod.SpeedrunTool.RoomTimer;
using Celeste.Mod.SpeedrunTool.SaveLoad;
using System;

namespace Celeste.Mod.WonderMods.Streaks;
public static class StreakCounter {
    private static int Count { get; set; } = 0;
    private static int Best { get; set; } = 0;
    private static DateTime LastResetTime { get; set; } = DateTime.Now;

    public enum StreakCounterType {
        Off,
        Auto,
        Manual
    }
    public static void ResetCount(bool displayPopupMessage) {
        if ((0 != Count) && displayPopupMessage) {
            PopupMessageUtils.Show(string.Format("{0} ({1})", DialogIds.StreakCounterResetId.DialogClean(), Count), null);
        }
        Count = 0;
    }

    public static void ResetBest(bool displayPopupMessage) {
        Best = 0;
    }

    public static int IncrementCount(int increment = 1)
    {
        Count += increment; 
        if (Count >= Best) {
            Best = Count;
            PopupMessageUtils.Show(string.Format("{0}: {1}", DialogIds.StreakCounterId.DialogClean(), Count), null);
        }
        else {
            PopupMessageUtils.Show(string.Format("{0}: {1} ({2})", DialogIds.StreakCounterId.DialogClean(), Count, Best), null);
        }
        return Count;
    }
    
    public static int GetCount() {
        return Count;
    }
}

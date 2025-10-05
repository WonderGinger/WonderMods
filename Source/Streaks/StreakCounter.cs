using Celeste.Mod.SpeedrunTool.Message;

using System;

namespace Celeste.Mod.WonderMods.Streaks;
public static class StreakCounter {
    private static int Count { get; set; } = 0;
    private static int Best { get; set; } = 0;
    private static DateTime LastResetTime { get; set; } = DateTime.Now;

    public enum StreakCounterType {
        OFF,
        AUTO,
        MANUAL
    }

    private static void PopupMessage(string message) {
        PopupMessageUtils.Show(message, null);
        WonderModsModule.WonderLog(message);
    }

    public static void ResetCount(bool displayPopupMessage) {
        if ((0 != Count) && displayPopupMessage) {
            PopupMessage(string.Format("{0} ({1})", DialogIds.StreakCounterResetId.DialogClean(), Count));
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
            PopupMessage(string.Format("{0}: {1}", DialogIds.StreakCounterId.DialogClean(), Count));
        }
        else {
            PopupMessage(string.Format("{0}: {1} ({2})", DialogIds.StreakCounterId.DialogClean(), Count, Best));
        }
        return Count;
    }

    public static int DecrementCount(int decrement = 1)
    {
        Count -= decrement;
        if (Count < 0) Count = 0;
        PopupMessage(string.Format("{0}: {1} ({2})", DialogIds.StreakCounterId.DialogClean(), Count, Best));
        return Count;
    }

    public static int GetCount() {
        return Count;
    }
}


using System;
using System.Collections.Generic;
using System.Text;
using static Celeste.Mod.WonderMods.WonderModsModule;

namespace Celeste.Mod.WonderMods.Streaks;

public class StreakState
{
    public int Count { get; set; } = 0;
    public int Best { get; set; } = 0;
    public List<long> BestRoomTimes = new([]);
    public List<long> CurRoomTimes = new([]);

    public StreakState Clone()
    {
        return new StreakState()
        {
            Count = this.Count,
            Best = this.Best,
            BestRoomTimes = this.BestRoomTimes,
            CurRoomTimes = this.CurRoomTimes,
        };
    }
}

public static class StreakCounter 
{
    private static StreakState Cur = new();
    private static LinkedList<StreakState> State = new([new StreakState()]);

    public enum StreakCounterType 
    {
        OFF,
        AUTO,
        MANUAL
    }

    public static void ResetCount(bool displayPopupMessage) 
    {
        int streakCount = GetCount();
        if ((0 != streakCount) && displayPopupMessage) {
            PopupMessage(string.Format("{0} ({1})", DialogIds.StreakCounterResetId.DialogClean(), streakCount));
        }
        Cur.CurRoomTimes.Clear();
        AddCount(0);
    }

    public static void ResetBest(bool displayPopupMessage)
    {
        Cur.BestRoomTimes.Clear();
        AddBest(0);
    }

    public static void Reset(bool displayPopupMessage) 
    {
        Cur = new StreakState();
        State = new([new StreakState()]);
    }

    private static void AddBest(int best) 
    {
        Cur.Best = best;
        Cur.CurRoomTimes.Add(StreakManager.LastRoomTime);
        Cur.BestRoomTimes.Clear();
        int idx = 0;
        foreach (long time in Cur.CurRoomTimes)
        {
            Cur.BestRoomTimes.Add(time);
            idx++;
            WonderLog($"{idx},{FormatTime(time)}");
        }
        AddState();
    }

    private static void AddCount(int count) 
    {
        Cur.Count = count;
        Cur.CurRoomTimes.Add(StreakManager.LastRoomTime);
        AddState();
    }

    private static void AddState()
    {
        State.AddFirst(Cur.Clone());
        if (State.Count > 32) {
            State.RemoveLast();
        }
    }

    public static int GetCount() 
    {
        return State.First.Value.Count;
    }

    public static int GetBest() 
    {
        return State.First.Value.Best;
    }

    public static List<long> GetBestRoomTimes()
    {
        return State.First.Value.BestRoomTimes;
    }

    public static int IncrementCount(int increment = 1)
    {
        int streakCount = GetCount() + increment;
        if (streakCount > 0xffff) streakCount = 0xffff;
        if (streakCount >= GetBest()) {
            Cur.Count = streakCount;
            Cur.Best = streakCount;
            AddBest(streakCount);
            PopupMessage(string.Format("{0}: {1}", DialogIds.StreakCounterId.DialogClean(), streakCount));
        }
        else {
            AddCount(streakCount);
            PopupMessage(string.Format("{0}: {1} ({2})", DialogIds.StreakCounterId.DialogClean(), streakCount, GetBest()));
        }
        return streakCount;
    }

    public static int DecrementCount(int decrement = 1)
    {
        int streakCount = GetCount() - decrement; 
        if (streakCount < 0) streakCount = 0;
        AddCount(streakCount);
        PopupMessage(string.Format("{0}: {1} ({2})", DialogIds.StreakCounterId.DialogClean(), streakCount, GetBest()));
        return streakCount;
    }

    public static bool UndoCount()
    {
        if (State.Count <= 1)
        {
            return false;
        }
        State.RemoveFirst();
        Cur = State.First.Value.Clone(); 

        PopupMessage(string.Format("{0}: {1} ({2})", DialogIds.StreakCounterId.DialogClean(), GetCount(), GetBest()));
        return true;
    }

    public static string FormatTime(long time)
    {
        TimeSpan timeSpan = TimeSpan.FromTicks(time);
        return timeSpan.ToString(timeSpan.TotalSeconds < 60 ? "s\\.fff" : "m\\:ss\\.fff");
    }

    public static void ExportStreakTimes()
    {
        List<long> bestRoomTimes = GetBestRoomTimes();
        if (bestRoomTimes.Count == 0)
        {
            PopupMessage("Unable to export times best times");
            return;
        }
        StringBuilder sb = new();

        // Header row
        sb.Append("Streak,Segment");

        for (int i = 0; i < bestRoomTimes.Count; i++)
        {
            WonderLog($"Best Room Time: {i+1} {bestRoomTimes[i]}");
            sb.Append($"\n{i + 1},{FormatTime(bestRoomTimes[i])}");
        }

        TextInput.SetClipboardText(sb.ToString());
        PopupMessage("Streak times exported");
    }
}

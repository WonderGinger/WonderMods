using MonoMod.ModInterop;
using System;
using Celeste.Mod.SpeedrunTool.ModInterop;
using System.Collections.Generic;
using Celeste.Mod.SpeedrunTool.RoomTimer;

namespace Celeste.Mod.WonderMods.Integration;


[ModImportName("SpeedrunTool.RoomTimer")]
public static class RoomTimerIntegration
{
    public static Func<bool> RoomTimerIsCompleted;
    public static Func<long> GetRoomTime;
    internal static void Load()
    {
        typeof(RoomTimerIntegration).ModInterop();
    }
    internal static void Unload() { }
}

[ModImportName("SpeedrunTool.SaveLoad")]
public static class SaveLoadIntegration
{
    public static Func<Action<Dictionary<Type, Dictionary<string, object>>, Level>,
        Action<Dictionary<Type, Dictionary<string, object>>, Level>, Action,
        Action<Level>, Action<Level>, Action, object> RegisterSaveLoadAction;

    internal static void Load()
    {
        typeof(SaveLoadIntegration).ModInterop();
    }

    internal static void Unload() 
    {
    }
}
using MonoMod.ModInterop;
using System;
using Celeste.Mod.SpeedrunTool.ModInterop;
using Celeste.Mod.SpeedrunTool.RoomTimer;
using System.Collections.Generic;

namespace Celeste.Mod.WonderMods.Integration
{
    public static class SpeedrunToolIntegration
    {
        private static object SaveLoadActionInstance = null;

        internal static void Load() 
        {
            SaveLoadActionInstance = SpeedrunToolInterop.SaveLoadExports.RegisterSaveLoadAction(OnSaveState, OnLoadState, OnClearState, OnBeforeSaveState, OnBeforeLoadState, null);
        }

        private static void OnBeforeLoadState(Level level)
        {
            WonderModsModule.WonderLog("Before load state hook");
            Streaks.StreakManager.OnBeforeLoadState();
        }

        private static void OnBeforeSaveState(Level level)
        {
            WonderModsModule.WonderLog("Before save state hook");
            Streaks.StreakManager.OnBeforeSaveState();
        }

        private static void OnClearState()
        {
            WonderModsModule.WonderLog("Clear state hook");
            Streaks.StreakManager.OnClearState();
        }

        private static void OnLoadState(Dictionary<Type, Dictionary<string, object>> dictionary, Level level)
        {
            WonderModsModule.WonderLog("Load state hook");
            Streaks.StreakManager.OnLoadState();
        }

        private static void OnSaveState(Dictionary<Type, Dictionary<string, object>> dictionary, Level level)
        {			
            WonderModsModule.WonderLog("Save state hook");
            Streaks.StreakManager.OnSaveState();
        }



        internal static void Unload() 
        {
            if (SaveLoadActionInstance != null)
            {
                SpeedrunToolInterop.SaveLoadExports.Unregister(SaveLoadActionInstance);
            }
        }
    }
}
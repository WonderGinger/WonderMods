using Celeste.Mod.WonderMods.Integration;
using Celeste.Mod.WonderMods.Streaks;

using Microsoft.Xna.Framework;
using Monocle;
using System;
using MonoMod.ModInterop;
using System.IO;

namespace Celeste.Mod.WonderMods
{
    public class WonderModsModule : EverestModule
    {
        public static WonderModsModule Instance { get; private set; }

        public override Type SettingsType => typeof(WonderModsModuleSettings);
        public static WonderModsModuleSettings Settings => (WonderModsModuleSettings)Instance._Settings;
        public override Type SessionType => typeof(WonderModsModuleSession);
        public static WonderModsModuleSession Session => (WonderModsModuleSession)Instance._Session;
        public static readonly string REPLAY_ROOT = Path.Combine(Everest.PathGame, "WonderModsRoot");

        public WonderModsModule()
        {
            Instance = this;
            
#if DEBUG
            // debug builds use verbose logging
            Logger.SetLogLevel(nameof(WonderModsModule), LogLevel.Verbose);
#else
            // release builds use info logging to reduce spam in log files
            Logger.SetLogLevel(nameof(WonderModsModule), LogLevel.Info);
#endif
        }
        private void AddStreaksCounterEntity(Level level, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            level.Add(new StreakCounterEntity());
        }

        public override void Load()
        {
            On.Monocle.Engine.Update += Engine_Update;
            //Everest.Events.Level.OnLoadLevel += AddStreaksCounterEntity;
            typeof(RoomTimerIntegration).ModInterop();
            typeof(SaveLoadIntegration).ModInterop();
            SaveLoadIntegration.RegisterSaveLoadAction(StreakManager.OnSaveState, StreakManager.OnLoadState, StreakManager.OnClearState, StreakManager.OnBeforeSaveState, StreakManager.OnBeforeLoadState, null);
        }

        private void Engine_Update(On.Monocle.Engine.orig_Update orig, Engine self, GameTime gameTime)
        {
            orig(self, gameTime);
            //if (!Settings.Enabled) return;
            if (Settings.KeyStreakIncrement.Pressed) StreakManager.ResetHotkey();
            if (Settings.KeyStreakReset.Pressed) StreakManager.IncrementHotkey();
        }

        public override void Unload()
        {
            On.Monocle.Engine.Update -= Engine_Update;
            //Everest.Events.Level.OnLoadLevel -= AddStreaksCounterEntity;
        }

        public override void CreateModMenuSection(TextMenu menu, bool inGame, FMOD.Studio.EventInstance snapshot)
		{
            base.CreateModMenuSection(menu, inGame, snapshot);

		}
        public static void WonderLog(string s)
        {
            Logger.Log(LogLevel.Debug, nameof(WonderModsModule), s);
        }
    }
}

using Celeste.Mod.WonderMods.Integration;
using Celeste.Mod.WonderMods.ReturnToMapSplitButton;
using Celeste.Mod.WonderMods.Streaks;
using Celeste.Mod.SpeedrunTool.Message;

using Microsoft.Xna.Framework;
using Monocle;
using System;
using MonoMod.ModInterop;
using System.IO;
using static Celeste.TextMenuExt;

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
        private object SaveLoadInstance = null;

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
            SaveLoadInstance = SaveLoadIntegration.RegisterSaveLoadAction(StreakManager.OnSaveState, StreakManager.OnLoadState, StreakManager.OnClearState, StreakManager.OnBeforeSaveState, StreakManager.OnBeforeLoadState, null);
            Everest.Events.Level.OnCreatePauseMenuButtons += Level_OnCreatePauseMenuButtons;
        }

        private void Engine_Update(On.Monocle.Engine.orig_Update orig, Engine self, GameTime gameTime)
        {
            orig(self, gameTime);
            //if (!Settings.Enabled) return;
            ReturnToMapTimer.Update();
            if ((Settings.EnableStreaks != StreakCounter.StreakCounterType.OFF)
                && (Engine.Scene is Level level) && !(level.Paused))
            {
                if (Settings.KeyStreakIncrement.Pressed) StreakManager.IncrementHotkey();
                //if (Settings.KeyStreakDecrement.Pressed) StreakManager.DecrementHotkey(); 
                if (Settings.KeyStreakReset.Pressed) StreakManager.ResetHotkey();
                if (Settings.KeyStreakUndo.Pressed) StreakManager.UndoHotkey();
                if (Settings.KeyStreakExport.Pressed) StreakManager.ExportHotkey();
                StreakManager.Update();
            }
        }

        public override void Unload()
        {
            On.Monocle.Engine.Update -= Engine_Update;
            SaveLoadIntegration.Unregister(SaveLoadInstance);
            Everest.Events.Level.OnCreatePauseMenuButtons -= Level_OnCreatePauseMenuButtons;
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

        public static void PopupMessage(string message) {
            PopupMessageUtils.Show(message, null);
            WonderLog(message);
        }

        private void Level_OnCreatePauseMenuButtons(Level level, TextMenu menu, bool minimal)
        {
            if (!Settings.ShowReturnToMapSplitButton) return;
            MainMenuReturnToMapSplitButton button = new(Dialog.Get(DialogIds.ReturnToMapSplitButtonId));
            button.Pressed(() =>
            {
                button.PressedHandler(level, menu);
            });
            EaseInSubHeaderExt descriptionText = new(Dialog.Get(DialogIds.ReturnToMapSplitButtonDescId), false, menu, null)
            {
                HeightExtra = 0f
            };
            menu.Add(button);
            menu.Add(descriptionText);
            button.OnEnter = () => descriptionText.FadeVisible = true;
            button.OnLeave = () => descriptionText.FadeVisible = false;
        }
    }
}

using Monocle;
using static Celeste.TextMenu;
using Celeste.Mod.SpeedrunTool.RoomTimer;
using System;

namespace Celeste.Mod.WonderMods.ReturnToMapSplitButton;
public class MainMenuReturnToMapSplitButton : Button
{
    public MainMenuReturnToMapSplitButton(string label) : base(label)
    {
        ConfirmSfx = SFX.ui_main_message_confirm;
    }
    
    public void PressedHandler(Level level, TextMenu menu)
    {
        if (level == null) return;
        ReturnToMapSplitMenu rtmSplitMenu = new(menu);
        menu.Focused = false;
        menu.Alpha = 0f;
        Engine.Scene.Add(rtmSplitMenu);
        Engine.Scene.OnEndOfFrame += () => Engine.Scene.Entities.UpdateLists();
    }
}

public class ReturnToMapSplitMenu : TextMenu
{
    public ReturnToMapSplitMenu(TextMenu parentMenu)
    {
        Level level = Engine.Scene as Level;
        OnCancel = () =>
        {
            Close();
            Audio.Play(SFX.ui_main_button_back);
        };
        OnESC = OnPause = () =>
        {
            Close();
            level.Unpause();
        };
        OnClose = () =>
        {
            parentMenu.Focused = true; 
            parentMenu.Alpha = 1.0f; 
        };

        Add(new Header(Dialog.Get(DialogIds.ReturnToMapSplitMenuHeaderId)));

        Button confirmButton = new("Yes");
        confirmButton.Pressed(() =>
        {
            ReturnToMapTimer.HandleReturnToMapTimerButtonPressed();
            Close();
            level.Unpause();
        });

        Button cancelButton = new("Cancel");
        cancelButton.Pressed(() =>
        {
            OnCancel();
        });

        Add(confirmButton);
        Add(cancelButton);
    }
}

public static class ReturnToMapTimer 
{
    private static int counter = 0;
    private static bool pressed = false;
    private const int RTM_FADEOUT_FRAMES = 31;

    // CelesteTAS info hud function https://github.com/EverestAPI/CelesteTAS-EverestInterop/blob/ae25bf3f2fa931d362c3a321c2cf8dae58d2eb28/CelesteTAS-EverestInterop/Source/TAS/GameInfo.cs#L546
    internal static int ToCeilingFrames(this float timer) {
        if (timer <= 0.0f) {
            return 0;
        }

        float frames = MathF.Ceiling(timer / Engine.DeltaTime);
        return float.IsInfinity(frames) || float.IsNaN(frames) ? int.MaxValue : (int) frames;
    }

    public static void HandleReturnToMapTimerButtonPressed()
    {
        Player player = (Engine.Scene as Level).Tracker.GetEntity<Player>();

        // CelesteTAS info hud format https://github.com/EverestAPI/CelesteTAS-EverestInterop/blob/ae25bf3f2fa931d362c3a321c2cf8dae58d2eb28/CelesteTAS-EverestInterop/Source/TAS/GameInfo.cs#L307
        Follower? firstRedBerryFollower = player.Leader.Followers.Find(follower => follower.Entity is Strawberry {Golden: false});
        if (firstRedBerryFollower?.Entity is Strawberry firstRedBerry) {
            float collectTimer = firstRedBerry.collectTimer;
            if (collectTimer <= 0.15f) {
                int collectFrames = (0.15f - collectTimer).ToCeilingFrames();
                if (collectTimer >= 0f) {
                    WonderModsModule.PopupMessage($"Berry({collectFrames}) ");
                } else {
                    int additionalFrames = Math.Abs(collectTimer).ToCeilingFrames();
                    WonderModsModule.PopupMessage($"Berry({collectFrames - additionalFrames}+{additionalFrames}) ");
                }
            }
            return;
        }

        pressed = true;
        counter = 0;
    }

    public static void Update()
    {
        if (pressed)
        {
            counter++;
            if (counter >= RTM_FADEOUT_FRAMES)
            {
                pressed = false;
                counter = 0;
                RoomTimerManager.UpdateTimerState();
            }
        }
        else
        {
            counter = 0;
        }
    }
}

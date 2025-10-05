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
            Audio.Play(SFX.ui_main_button_back);
            Close();
        };
        OnESC = () =>
        {
            level.Unpause();
            Close();
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
            level.Unpause();
            Close();
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
    public static void HandleReturnToMapTimerButtonPressed()
    {
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

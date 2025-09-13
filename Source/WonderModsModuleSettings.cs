using Microsoft.Xna.Framework.Input;
using Celeste.Mod.WonderMods.Streaks;

namespace Celeste.Mod.WonderMods;
[SettingName(DialogIds.WonderMods)]
public class WonderModsModuleSettings : EverestModuleSettings
{
    public static WonderModsModuleSettings Instance { get; private set; }
    public WonderModsModuleSettings()
    {
        Instance = this;
    }

    //public bool Enabled { get; set; } = true;

    #region Streaks

    [SettingName(DialogIds.EnableStreaksId)]
    public StreakCounter.StreakCounterType EnableStreaks { get; set; } = StreakCounter.StreakCounterType.Auto;
    //[SettingName(DialogIds.AlwaysShowStreakCounterId)]
    //public bool AlwaysShowStreakCounter { get; set; } = false;

    #endregion

    #region Hotkey

    [SettingName(DialogIds.KeyStreakIncrementId)]
    [DefaultButtonBinding(0, Keys.OemPlus)]
    public ButtonBinding KeyStreakIncrement { get; set; } = new(0, Keys.OemPlus);

    [SettingName(DialogIds.KeyStreakDecrementId)]
    [DefaultButtonBinding(0, Keys.OemMinus)]
    public ButtonBinding KeyStreakDecrement { get; set; } = new(0, Keys.OemMinus);

    [SettingName(DialogIds.KeyStreakResetId)]
    [DefaultButtonBinding(0, Keys.Delete)]
    public ButtonBinding KeyStreakReset { get; set; } = new(0, Keys.Delete);

    #endregion


}

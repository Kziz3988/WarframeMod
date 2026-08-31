
using BaseLib.Config;

namespace WarframeMod.Code.Config;

internal class WarframeModConfig : SimpleModConfig
{
    [ConfigHoverTip]
    public static bool EnableMultiplayerRebalance { get; set; } = true;

    [ConfigHoverTip]
    public static bool ReplacePrismaticGem { get; set; } = false;
}
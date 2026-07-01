
using BaseLib.Config;

namespace WarframeMod.Code.Config;

internal class WarframeModConfig : SimpleModConfig
{
    public static bool EnableMultiplayerRebalance { get; set; } = true;
}
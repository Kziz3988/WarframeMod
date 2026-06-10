using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
namespace WarframeMod.Code;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "WarframeMod";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Log.Info("[Warframe Mod] Initializing...");
        Harmony harmony = new(ModId);
        harmony.PatchAll();
        Log.Info("[Warframe Mod] Loaded successfully.");
    }
}

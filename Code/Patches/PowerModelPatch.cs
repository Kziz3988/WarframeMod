using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(PowerModel))]
public static class PowerModelPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(PowerModel.AllowNegative), MethodType.Getter)]
    static bool AllowNegativePatch(PowerModel __instance, ref bool __result)
    {
        if (__instance is DrawCardsNextTurnPower)
        {
            __result = true;
            return false;
        }
        return true;
    }
}
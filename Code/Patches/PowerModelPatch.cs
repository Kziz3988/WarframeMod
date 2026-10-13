using System;
using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(PowerModel))]
public static class PowerModelPatch
{
    private static HashSet<Type>? _allowNegativePowers;

    public static HashSet<Type> AllowNegativePowers
    {
        get
        {
            if (_allowNegativePowers == null)
            {
                _allowNegativePowers = [
                    typeof(DrawCardsNextTurnPower),
                    typeof(EnergyNextTurnPower)
                ];
            }
            return _allowNegativePowers;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(PowerModel.AllowNegative), MethodType.Getter)]
    static bool AllowNegativePatch(PowerModel __instance, ref bool __result)
    {
        if (AllowNegativePowers.Contains(__instance.GetType()))
        {
            __result = true;
            return false;
        }
        return true;
    }
}
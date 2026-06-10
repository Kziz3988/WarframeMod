using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using WarframeMod.Code.Relics.Ancient;
using WarframeMod.Code.Relics.Starter;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(TouchOfOrobas))]
public static class TouchOfOrobasPatch
{
    private static Dictionary<ModelId, RelicModel> _cachedCustomUpgrades;
    
    [HarmonyPostfix]
    [HarmonyPatch("get_RefinementUpgrades")]
    static void GetRefinementUpgradesPatch(ref Dictionary<ModelId, RelicModel> __result)
    {
        if (_cachedCustomUpgrades == null)
        {
            _cachedCustomUpgrades = new Dictionary<ModelId, RelicModel>(__result)
            {
                [ModelDb.Relic<ArcaneHusk>().Id] = ModelDb.Relic<ArcaneBarrier>()
            };
        }
        
        __result = _cachedCustomUpgrades;
    }
}
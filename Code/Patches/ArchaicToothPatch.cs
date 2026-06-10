using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using WarframeMod.Code.Cards.Ancient;
using WarframeMod.Code.Cards.Basic;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(ArchaicTooth))]
public static class ArchaicToothPatch
{
    private static Dictionary<ModelId, CardModel> _cachedCustomUpgrades;
    
    [HarmonyPostfix]
    [HarmonyPatch("get_TranscendenceUpgrades")]
    static void GetTranscendenceUpgradesPatch(ref Dictionary<ModelId, CardModel> __result)
    {
        if (_cachedCustomUpgrades == null)
        {
            _cachedCustomUpgrades = new Dictionary<ModelId, CardModel>(__result)
            {
                [ModelDb.Card<ExaltedBlade>().Id] = ModelDb.Card<ChromaticBlade>()
            };
        }
        
        __result = _cachedCustomUpgrades;
    }
}

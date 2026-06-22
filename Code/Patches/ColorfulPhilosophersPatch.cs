using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(ColorfulPhilosophers))]
public static class ColorfulPhilosophersPatch
{
    private static IEnumerable<CardPoolModel> _cachedCustomCardPools;
    
    [HarmonyPostfix]
    [HarmonyPatch("get_CardPoolColorOrder")]
    static void GetCardPoolColorOrderPatch(ref IEnumerable<CardPoolModel> __result)
    {
        if (_cachedCustomCardPools == null)
        {
            _cachedCustomCardPools = new List<CardPoolModel>(__result)
            {
                ModelDb.CardPool<WarframeModCardPool>()
            };
        }
        //CARD_POOL∴WARFRAMEMOD-WARFRAME_MOD_CARD_POOL
        
        __result = _cachedCustomCardPools;
    }
}

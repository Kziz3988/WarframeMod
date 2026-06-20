using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using WarframeMod.Code.Cards.Event;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(TrashHeap))]
public static class TrashHeapPatch
{
    private static CardModel[] _cachedCustomCards;
    
    [HarmonyPostfix]
    [HarmonyPatch("get_Cards")]
    static void GetCardsPatch(ref CardModel[] __result)
    {
        if (_cachedCustomCards == null)
        {
            var customCardList = new List<CardModel>(__result)
            {
                ModelDb.Card<Crystallize>(),
                ModelDb.Card<FinalStand>(),
                ModelDb.Card<SereneStorm>(),
                ModelDb.Card<WyrdScythes>(),
            };
            
            _cachedCustomCards = customCardList.ToArray();
        }
        
        __result = _cachedCustomCards;
    }
}
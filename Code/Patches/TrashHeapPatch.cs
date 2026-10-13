using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(TrashHeap))]
public static class TrashHeapPatch
{  
    [HarmonyPostfix]
    [HarmonyPatch("get_Cards")]
    static void GetCardsPatch(ref CardModel[] __result)
    {
        __result = __result.Concat(ModelDb.CardPool<WarframeTrashHeapCardPool>().AllCards).ToArray();
    }
}
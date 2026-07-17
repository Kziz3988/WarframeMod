using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using WarframeMod.Code.Relics.Ancient;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(nameof(NeowsBones))]
public static class NeowsBonesPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NeowsBones), "GetValidRelics")]
    static void GetValidRelicsPatch(NeowsBones __instance, Player owner, ref IEnumerable<RelicModel> __result)
    {
        if (owner == null || owner.Character.Id.ToString() != "CHARACTER.WARFRAMEMOD-EXCALIBUR")
        {
            return;
        }
        __result = __result.Select(r => r is LeafyPoultice ? ModelDb.Relic<NewLeafyPoultice>().ToMutable() : r);
        __result = __result.Select(r => r is NeowsTalisman ? ModelDb.Relic<NewNeowsTalisman>().ToMutable() : r);
    }
}
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using WarframeMod.Code.Character;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Relics.Ancient;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(nameof(NeowsBones))]
public static class NeowsBonesPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NeowsBones), "GetValidRelics")]
    static void GetValidRelicsPatch(NeowsBones __instance, Player owner, ref IEnumerable<RelicModel> __result)
    {
        if (owner == null || !owner.Character.IsWarframe())
        {
            return;
        }
        NewLeafyPoultice newLeafyPoultice = (NewLeafyPoultice)ModelDb.Relic<NewLeafyPoultice>().ToMutable();
        NewNeowsTalisman newNeowsTalisman = (NewNeowsTalisman)ModelDb.Relic<NewNeowsTalisman>().ToMutable();
        newLeafyPoultice.Strike = WarframeModCardPool.CustomStrikeAndDefend[owner.Character.Id].strike;
        newLeafyPoultice.Defend = WarframeModCardPool.CustomStrikeAndDefend[owner.Character.Id].defend;
        newNeowsTalisman.Strike = WarframeModCardPool.CustomStrikeAndDefend[owner.Character.Id].strike;
        newNeowsTalisman.Defend = WarframeModCardPool.CustomStrikeAndDefend[owner.Character.Id].defend;
        __result = __result.Select(r => r is LeafyPoultice ? newLeafyPoultice : r);
        __result = __result.Select(r => r is NeowsTalisman ? newNeowsTalisman : r);
    }
}
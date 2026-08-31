using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using WarframeMod.Code.Character;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Relics.Ancient;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(nameof(Neow))]
public static class NeowPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Neow), "CurseOptions", MethodType.Getter)]
    static void GetCurseOptionsPatch(Neow __instance, ref IEnumerable<EventOption> __result)
    {
        if (__instance.Owner == null || !__instance.Owner.Character.IsWarframe())
        {
            return;
        }

        var originalOptions = __result;
        var modifiedOptions = ReplaceRelicOption<LeafyPoultice, NewLeafyPoultice>(__instance, originalOptions, "NEOW.pages.DONE.CURSED.description");
        NewLeafyPoultice? newLeafyPoultice = (NewLeafyPoultice?)modifiedOptions.First(r => r.Relic?.GetType() == typeof(NewLeafyPoultice)).Relic;
        if (newLeafyPoultice != null)
        {
            newLeafyPoultice.Strike = WarframeModCardPool.CustomStrikeAndDefend[__instance.Owner.Character.Id].strike;
            newLeafyPoultice.Defend = WarframeModCardPool.CustomStrikeAndDefend[__instance.Owner.Character.Id].defend;
        }
        __result = modifiedOptions;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Neow), "NeowsTalismanOption", MethodType.Getter)]
    static bool GetNeowsTalismanOptionPatch(Neow __instance, ref EventOption __result)
    {
        if (__instance.Owner == null || !__instance.Owner.Character.IsWarframe())
        {
            return true;
        }
        var modifiedOption = __instance.RelicOption<NewNeowsTalisman>("INITIAL", "NEOW.pages.DONE.POSITIVE.description");
        NewNeowsTalisman? newNeowsTalisman = (NewNeowsTalisman?)modifiedOption.Relic;
        if (newNeowsTalisman != null)
        {
            newNeowsTalisman.Strike = WarframeModCardPool.CustomStrikeAndDefend[__instance.Owner.Character.Id].strike;
            newNeowsTalisman.Defend = WarframeModCardPool.CustomStrikeAndDefend[__instance.Owner.Character.Id].defend;
        }
        __result = modifiedOption;
        return false;
    }

    static IEnumerable<EventOption> ReplaceRelicOption<TOriginal, TReplacement>(Neow __instance, IEnumerable<EventOption> originalOptions, string description)
        where TOriginal : RelicModel, new()
        where TReplacement : RelicModel, new()
    {
        var result = new List<EventOption>();

        foreach (var option in originalOptions)
        {
            if (option.Relic is TOriginal)
            {
                var newOption = __instance.RelicOption<TReplacement>("INITIAL", description);
                result.Add(newOption);
            }
            else
            {
                result.Add(option);
            }
        }

        return result;
    }
}
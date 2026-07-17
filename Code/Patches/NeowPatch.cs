using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using WarframeMod.Code.Relics.Ancient;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(nameof(Neow))]
public static class NeowPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Neow), "CurseOptions", MethodType.Getter)]
    static void GetCurseOptionsPatch(Neow __instance, ref IEnumerable<EventOption> __result)
    {
        if (__instance.Owner == null || __instance.Owner.Character.Id.ToString() != "CHARACTER.WARFRAMEMOD-EXCALIBUR")
        {
            return;
        }

        var originalOptions = __result;
        var modifiedOptions = ReplaceRelicOption<LeafyPoultice, NewLeafyPoultice>(__instance, originalOptions, "NEOW.pages.DONE.CURSED.description");
        __result = modifiedOptions;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Neow), "NeowsTalismanOption", MethodType.Getter)]
    static bool GetNeowsTalismanOptionPatch(Neow __instance, ref EventOption __result)
    {
        if (__instance.Owner == null || __instance.Owner.Character.Id.ToString() != "CHARACTER.WARFRAMEMOD-EXCALIBUR")
        {
            return true;
        }
        __result = RelicOption<NewNeowsTalisman>(__instance, "INITIAL", "NEOW.pages.DONE.POSITIVE.description");
        return false;
    }

    static EventOption RelicOption<T>(Neow __instance, string page, string descriptionOverride) where T : RelicModel, new()
    {
        var _relicOptionMethod = typeof(Neow).GetMethod(
            "RelicOption", 
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [typeof(string), typeof(string)],
            null
        );
            
        if (_relicOptionMethod == null)
        {
            throw new InvalidOperationException("Cannot find RelicOption method");
        }
        
        var genericMethod = _relicOptionMethod.MakeGenericMethod(typeof(T));
        return (EventOption)genericMethod.Invoke(__instance, [page, descriptionOverride]);
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
                var newOption = RelicOption<TReplacement>(__instance, "INITIAL", description);
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
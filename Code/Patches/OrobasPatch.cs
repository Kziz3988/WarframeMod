using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using WarframeMod.Code.Character;
using WarframeMod.Code.Config;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(nameof(Orobas))]
public static class OrobasPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Orobas), "SeaGlassOptions", MethodType.Getter)]
    static void SeaGlassOptionsPatch(Orobas __instance, ref IEnumerable<EventOption> __result)
    {
        if (__instance.Owner == null || !__instance.Owner.Character.IsWarframe())
        {
            __result = __result.Where(o => o.Relic is SeaGlass s && (s.CharacterId == ModelDb.Character<Excalibur>().Id || !s.CharacterId.IsWarframe()));
            return;
        }
        __result = __result.Where(o => o.Relic is SeaGlass s && !s.CharacterId.IsWarframe());
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Orobas), "GenerateInitialOptions")]
    static void GenerateInitialOptionsPatch(Orobas __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (__instance.Owner == null || !__instance.Owner.Character.IsWarframe())
        {
            return;
        }
        var option = __result.FirstOrDefault(o => o.Relic is SeaGlass s && s.CharacterId.IsWarframe());
        CharacterModel character = __instance.Rng.NextItem(__instance.Owner.UnlockState.Characters.Where(c => !c.IsWarframe())) ?? __instance.Owner.Character;
        SeaGlass seaGlass = (SeaGlass)ModelDb.Relic<SeaGlass>().ToMutable();
        seaGlass.CharacterId = character.Id;
        var modifiedOption = __instance.RelicOption(seaGlass);
        if (option != null)
        { 
            __result = __result.Select(o => o == option ? modifiedOption : o).ToList();
            return;
        }
        option = __result.FirstOrDefault(o => o.Relic is PrismaticGem s && __instance.Owner.Character.IsWarframe());
        if (option != null && WarframeModConfig.ReplacePrismaticGem)
        {
            __result = __result.Select(o => o == option ? modifiedOption : o).ToList();
            return;
        }
    }
}
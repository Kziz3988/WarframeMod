using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Encounters;

namespace WarframeMod.Code.Patches;
[HarmonyPatch(nameof(ModelDb))]
public static class ModelDbPatch
{
    private static IEnumerable<EncounterModel>? _cachedEventEncounters;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ModelDb), nameof(ModelDb.EventEncounters), MethodType.Getter)]
    static void GetEventEncountersPatch(ref IEnumerable<EncounterModel> __result)
    {
        if (_cachedEventEncounters == null)
        {
            _cachedEventEncounters = new List<EncounterModel>(__result)
            {
                ModelDb.Encounter<TheGrustragThree>(),
                ModelDb.Encounter<ZanukaHunterEvent>()
            };
        }
        
        __result = _cachedEventEncounters;
    }
}
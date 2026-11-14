using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Encounters;
using WarframeMod.Code.Config;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(ActModel))]
public static class ActModelPatch
{
    private static HashSet<Type>? _affectedActs;
    private static HashSet<Type>? _bossesToRemove;

    private static HashSet<Type> AffectedActs
    {
        get
        {
            if (_affectedActs == null)
            {
                _affectedActs = [
                    typeof(Overgrowth),
                    typeof(Underdocks),

                    typeof(Hive),

                    typeof(Glory)
                ];
            }
            return _affectedActs;
        }
    }

    private static HashSet<Type> BossesToRemove
    {
        get
        {
            if (_bossesToRemove == null)
            {
                _bossesToRemove = [
                    typeof(CeremonialBeastBoss),
                    typeof(TheKinBoss),
                    typeof(VantomBoss),

                    typeof(LagavulinMatriarchBoss),
                    typeof(SoulFyshBoss),
                    typeof(WaterfallGiantBoss),
                    
                    typeof(KaiserCrabBoss),
                    typeof(KnowledgeDemonBoss),
                    typeof(TheInsatiableBoss),

                    typeof(AeonglassBoss),
                    typeof(QueenBoss),
                    typeof(TestSubjectBoss),
                ];
            }
            return _bossesToRemove;
        }
    }

    static IEnumerable<MethodBase> TargetMethods()
    {
        foreach (var actType in AffectedActs)
        {
            var method = actType.GetMethod(
                nameof(ActModel.GenerateAllEncounters),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (method != null)
            {
                yield return method;
            }
        }
    }

    [HarmonyPostfix]
    static void GenerateAllEncountersPatch(ActModel __instance, ref IEnumerable<EncounterModel> __result)
    {
        if (!WarframeModConfig.RemoveVanillaBosses)
        {
            return;
        }

        if (!AffectedActs.Any(t => t.IsInstanceOfType(__instance)))
        {
            return;
        }

        var filtered = __result.Where(e => !BossesToRemove.Any(t => t.IsInstanceOfType(e))).ToList();

        __result = filtered;
    }
}
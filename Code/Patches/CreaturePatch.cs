using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(Creature))]
public static class CreaturePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Creature.StunInternal))]
    static bool StunInternalPatch(Creature __instance, Func<IReadOnlyList<Creature>, Task> stunMove, string? nextMoveId)
    {
        OverguardPower? overguard = __instance.GetPower<OverguardPower>();
        if (overguard == null || overguard.Amount <= 0)
        {
            return true;
        }
        PowerCmd.Decrement(overguard);
        return false;
    }
}
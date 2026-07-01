
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(Entomancer))]
public static class EntomancerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("SpitMove")]
    private static bool SpitMovePrefix(Entomancer __instance, IReadOnlyList<Creature> targets, ref Task __result)
    {
        PowerModel? personalHivePower = __instance.Creature.GetPower<PersonalHivePower>();
        
        if (personalHivePower == null)
        {
            __result = FixedSpitMove(__instance, targets);
            return false;
        }

        return true;
    }

    private static async Task FixedSpitMove(Entomancer __instance, IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(__instance.Creature, "Cast", 0.5f);   
        await PowerCmd.Apply<PersonalHivePower>(new ThrowingPlayerChoiceContext(), __instance.Creature, 1m, __instance.Creature, null);
		await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), __instance.Creature, 1m, __instance.Creature, null);
    }
}
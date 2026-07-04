using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Enchantments;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(PlayCardAction))]
public static class PlayCardActionPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayCardAction.TargetId), MethodType.Getter)]
    static bool TargetIdPatch(PlayCardAction __instance, ref uint? __result)
    {
        CardModel card = __instance.NetCombatCard.ToCardModel();
        if (card.TargetType == TargetType.AnyEnemy && card.Enchantment is HeavyCaliber)
        {
            var combatState = card.CombatState ?? card.Owner.Creature.CombatState;
            if (combatState == null)
            {
                return true;
            }
            var target = card.Owner.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
            if (target == null)
            {
                return true;
            }
            __result = target.CombatId;
            return false;
        }
        return true;
    }
}
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using System.Linq;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using WarframeMod.Code.Powers.Debuff;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace WarframeMod.Code.Patches
{
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.IsValidTarget))]
    public static class CardModel_IsValidTarget_Patch
    {
        static bool Prefix(CardModel __instance, Creature? target, ref bool __result)
        {
            if (__instance.CombatState == null)
            {
                return true;
            }

            if (__instance.TargetType == TargetType.AnyEnemy)
            {
                List<Creature> allEnemies = __instance.CombatState.HittableEnemies.ToList();
                List<Creature> voidPowerEnemies = allEnemies.Where(e => e.GetPower<VoidPower>() != null).ToList();

                if (voidPowerEnemies.Any())
                {
                    if (target != null && voidPowerEnemies.Contains(target))
                    {
                        return true;
                    }
                    else
                    {
                        LocString playerDialogueLine = new LocString("combat_messages", "WARFRAMEMOD-BLOCKED_BY_VOID");
			            if (playerDialogueLine != null)
			            {
				            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NThoughtBubbleVfx.Create(playerDialogueLine.GetFormattedText(), __instance.Owner.Creature, 1.0));
			            }
                        __result = false;
                        return false;
                    }
                }
            }

            if (__instance.TargetType == TargetType.AnyAlly)
            {
                List<Creature> allAllies = __instance.CombatState.GetTeammatesOf(__instance.Owner.Creature)
                    .Where(c => c.IsAlive)
                    .ToList();
                
                List<Creature> voidPowerAllies = allAllies.Where(a => a.GetPower<VoidPower>() != null).ToList();

                if (voidPowerAllies.Any())
                {
                    if (target != null && voidPowerAllies.Contains(target))
                    {
                        return true;
                    }
                    else
                    {
                        LocString playerDialogueLine = new LocString("combat_messages", "WARFRAMEMOD-BLOCKED_BY_VOID");
			            if (playerDialogueLine != null)
			            {
				            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NThoughtBubbleVfx.Create(playerDialogueLine.GetFormattedText(), __instance.Owner.Creature, 1.0));
			            }
                        __result = false;
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
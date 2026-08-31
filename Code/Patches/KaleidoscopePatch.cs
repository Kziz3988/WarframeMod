using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(nameof(Kaleidoscope))]
public static class KaleidoscopePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Kaleidoscope), nameof(Kaleidoscope.AfterObtained))]
    static bool AfterObtainedPatch(Kaleidoscope __instance, ref Task __result)
    {
        if (!__instance.Owner.Character.IsWarframe())
        {
            return true;
        }
        __result = GenerateReward(__instance);
        return false;
    }

    static async Task GenerateReward(Kaleidoscope __instance)
    {
        List<Reward> list = [];
		CardCreationOptions rerollOptions = CardCreationOptions.ForNonCombatWithDefaultOdds([]);
		for (int i = 0; i < __instance.DynamicVars.Cards.IntValue; i++)
		{
			List<CardModel> list2 = [];
			IEnumerable<CardPoolModel> enumerable = __instance.Owner.UnlockState.CharacterCardPools.Where(p => !p.Title.IsWarframe()).ToList().StableShuffle(__instance.Owner.RunState.Rng.Niche)
				.Take(3);
			foreach (CardPoolModel item in enumerable)
			{
				CardCreationOptions options = new CardCreationOptions([item], CardCreationSource.Other, CardRarityOddsType.RegularEncounter).WithFlags(CardCreationFlags.NoCardPoolModifications);
				list2.Add(CardFactory.CreateForReward(__instance.Owner, 1, options).First().Card);
			}
			list.Add(new CardReward(list2, CardCreationSource.Other, __instance.Owner, rerollOptions));
		}
		await RewardsCmd.OfferCustom(__instance.Owner, list);
    }
}
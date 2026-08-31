using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using WarframeMod.Code.Cards.Ancient;
using WarframeMod.Code.Cards.Basic;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Patches;

[HarmonyPatch(typeof(ArchaicTooth))]
public static class ArchaicToothPatch
{
    private static Dictionary<ModelId, CardModel> _cachedCustomUpgrades;
    
    [HarmonyPostfix]
    [HarmonyPatch("get_TranscendenceUpgrades")]
    static void GetTranscendenceUpgradesPatch(ref Dictionary<ModelId, CardModel> __result)
    {
        if (_cachedCustomUpgrades == null)
        {
            _cachedCustomUpgrades = new Dictionary<ModelId, CardModel>(__result)
            {
                [ModelDb.Card<Discharge>().Id] = ModelDb.Card<Capacitance>(),
                [ModelDb.Card<ExaltedBlade>().Id] = ModelDb.Card<ChromaticBlade>(),
                [ModelDb.Card<Pull>().Id] = ModelDb.Card<GreedyPull>()
            };
        }
        
        __result = _cachedCustomUpgrades;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(ArchaicTooth.AfterObtained))]
    static bool AfterObtainedPatch(ArchaicTooth __instance, ref Task __result)
    {
        if (__instance.Owner.Character.Id == ModelDb.Character<Mag>().Id)
        {
            __result = TransformAllCards(__instance, ModelDb.Card<Pull>().Id);
            return false;
        }
        return true;
    }

    static async Task TransformAllCards(ArchaicTooth __instance, ModelId cardId)
    {
        CardModel? original = GetOriginalCard(__instance, cardId);
        while (original != null)
        {
            await CardCmd.Transform(original, GetReplacementCard(__instance, original));
            original = GetOriginalCard(__instance, cardId);
        }
    }

    static CardModel? GetOriginalCard(ArchaicTooth __instance, ModelId cardId)
    {
        return __instance.Owner.Deck.Cards.FirstOrDefault(c => c.Id == cardId);
    }

    static CardModel GetReplacementCard(ArchaicTooth __instance, CardModel starterCard)
    {
       	if (_cachedCustomUpgrades.TryGetValue(starterCard.Id, out CardModel value))
		{
			CardModel card = starterCard.Owner.RunState.CreateCard(value, starterCard.Owner);
			if (starterCard.IsUpgraded)
			{
				CardCmd.Upgrade(card);
			}
			if (starterCard.Enchantment != null)
			{
				EnchantmentModel enchantmentModel = (EnchantmentModel)starterCard.Enchantment.MutableClone();
				CardCmd.Enchant(enchantmentModel, card, enchantmentModel.Amount);
			}
			return card;
		}
		return __instance.Owner.RunState.CreateCard<Doubt>(starterCard.Owner);
    }
}

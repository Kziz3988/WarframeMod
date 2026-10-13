using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Relics.Ancient;

public sealed class TemporalDust : WarframeModRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2),
        new GoldVar(50)
    ];

	private bool TrashHeapContainsCard(CardModel card)
    {
        var prop = typeof(TrashHeap).GetProperty("Cards", BindingFlags.NonPublic | BindingFlags.Static);
		if (prop == null)
        {
            throw new InvalidOperationException("Cannot find Cards property");
        }
        return ((CardModel[])prop.GetValue(null)).Select(c => c.GetType()).Contains(card.GetType());
    }

    public override async Task AfterObtained()
	{
		List<Reward> rewards = [];
		CardCreationOptions rerollOptions = CardCreationOptions.ForNonCombatWithDefaultOdds([]);
        CardPoolModel cardPool = ModelDb.CardPool<WarframeTrashHeapCardPool>();
		for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
		{
            CardCreationOptions options = new CardCreationOptions([cardPool], CardCreationSource.Other, CardRarityOddsType.Uniform).WithFlags(CardCreationFlags.NoCardPoolModifications);
            List<CardModel> reward = (from c in CardFactory.CreateForReward(base.Owner, 3, options) select c.Card).ToList();
			rewards.Add(new CardReward(reward, CardCreationSource.Other, base.Owner, rerollOptions));
		}
		await RewardsCmd.OfferCustom(base.Owner, rewards);
	}

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
	{
		CardPile? pile = card.Pile;
		if (pile != null && pile.Type == PileType.Deck && card.Owner == base.Owner && TrashHeapContainsCard(card))
		{
			Flash();
			await PlayerCmd.GainGold(base.DynamicVars.Gold.BaseValue, base.Owner);
		}
	}
}

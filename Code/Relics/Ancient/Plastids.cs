using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using WarframeMod.Code.Cards;
using WarframeMod.Code.Cards.Status;
using WarframeMod.Code.Events;

namespace WarframeMod.Code.Relics.Ancient;

public sealed class Plastids : WarframeModRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<InfestedTumor>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public override async Task AfterObtained()
	{
        CardModel original = PileType.Deck.GetPile(base.Owner).Cards
            .Where(c => c.IsTransformable).ToList()
            .StableShuffle(base.Owner.PlayerRng.Transformations)
			.First();
        CardModel replacement = base.Owner.RunState.CreateCard<InfestedTumor>(base.Owner);
        await WarframeModCard.TransformTo(original, replacement);
        
        List<Reward> rewards = [];
		CardCreationOptions rerollOptions = CardCreationOptions.ForNonCombatWithDefaultOdds([]);
        CardPoolModel cardPool = ModelDb.CardPool<EventCardPool>();
        HashSet<Type> helminthCards = Helminth.CommonCards.Concat(Helminth.UncommonCards).Concat(Helminth.RareCards).Select(c => c.GetType()).ToHashSet();
		for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
		{
            CardCreationOptions options = new CardCreationOptions([cardPool], CardCreationSource.Other, CardRarityOddsType.Uniform, c => helminthCards.Contains(c.GetType()))
                .WithFlags(CardCreationFlags.NoCardPoolModifications);
            List<CardModel> reward = (from c in CardFactory.CreateForReward(base.Owner, 3, options) select c.Card).ToList();
			rewards.Add(new CardReward(reward, CardCreationSource.Other, base.Owner, rerollOptions));
		}
		await RewardsCmd.OfferCustom(base.Owner, rewards);
	}
}


using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Runs;
using WarframeMod.Code.Cards;
using WarframeMod.Code.Cards.Event;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Events;

public sealed class Helminth : CustomEventModel
{
    public override string? CustomInitialPortraitPath => "helminth.png".EventBackgroundPath();

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new StringVar("CommonCardName"),
        new StringVar("UncommonCardName"),
        new StringVar("RareCardName")
    ];
    
    public override ActModel[] Acts => [ModelDb.Act<Hive>()];

	private static CardModel[] CommonCards =>
	[
		ModelDb.Card<Empower>(),
		ModelDb.Card<RebuildShield>()
    ];

    private static CardModel[] UncommonCards =>
	[
		ModelDb.Card<InfestedMobility>(),
        ModelDb.Card<SickeningPulse>()
    ];

    private static CardModel[] RareCards =>
	[
		ModelDb.Card<EnergizedMunitions>(),
		ModelDb.Card<ExpediteSuffering>()
    ];

    private (int common, int uncommon, int rare) CountCards(Player? player)
    {
        (int common, int uncommon, int rare) count = new(0, 0, 0);
        if (player == null)
        {
            return count;
        }
        foreach (CardModel card in player.Deck.Cards)
        {
            switch(card.Rarity)
            {
                case CardRarity.Common:
                    count.common++;
                    break;
                case CardRarity.Uncommon:
                    count.uncommon++;
                    break;
                case CardRarity.Rare:
                    count.rare++;
                    break;
            }
        }
        return count;
    }

    private bool HasCard((int common, int uncommon, int rare) count)
    {
        return count.common > 0 || count.uncommon > 0 || count.rare > 0;
    }

    public override bool IsAllowed(IRunState runState)
    {
        return runState.Players.All(p => HasCard(CountCards(p)));
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<EventOption> list = [];
		
        if (CountCards(base.Owner).common > 0)
		{
            CardModel commonCard = base.Owner.RunState.CreateCard(base.Rng.NextItem(CommonCards), base.Owner);
            ((StringVar)base.DynamicVars["CommonCardName"]).StringValue = WarframeModCard.GetCard(commonCard.GetType()).Title;
            list.Add(new EventOption(this, () => Inject(commonCard, CardRarity.Common), "WARFRAMEMOD-HELMINTH.pages.INITIAL.options.FEED_COMMON", new CardHoverTip(commonCard)));
		}
		else
		{
			list.Add(new EventOption(this, null, "WARFRAMEMOD-HELMINTH.pages.INITIAL.options.FEED_COMMON_LOCKED"));
		}

        if (CountCards(base.Owner).uncommon > 0)
		{
            CardModel uncommonCard = base.Owner.RunState.CreateCard(base.Rng.NextItem(UncommonCards), base.Owner);
			((StringVar)base.DynamicVars["UncommonCardName"]).StringValue = WarframeModCard.GetCard(uncommonCard.GetType()).Title;
            list.Add(new EventOption(this, () => Inject(uncommonCard, CardRarity.Uncommon), "WARFRAMEMOD-HELMINTH.pages.INITIAL.options.FEED_UNCOMMON", new CardHoverTip(uncommonCard)));
		}
		else
		{
			list.Add(new EventOption(this, null, "WARFRAMEMOD-HELMINTH.pages.INITIAL.options.FEED_UNCOMMON_LOCKED"));
		}

        if (CountCards(base.Owner).rare > 0)
		{
            CardModel rareCard = base.Owner.RunState.CreateCard(base.Rng.NextItem(RareCards), base.Owner);
			((StringVar)base.DynamicVars["RareCardName"]).StringValue = WarframeModCard.GetCard(rareCard.GetType()).Title;
            list.Add(new EventOption(this, () => Inject(rareCard, CardRarity.Rare), "WARFRAMEMOD-HELMINTH.pages.INITIAL.options.FEED_RARE", new CardHoverTip(rareCard)));
		}
		else
		{
			list.Add(new EventOption(this, null, "WARFRAMEMOD-HELMINTH.pages.INITIAL.options.FEED_RARE_LOCKED"));
		}

        return list;
    }

    private async Task Inject(CardModel card, CardRarity rarity)
    {
        CardModel original = (await CardSelectCmd.FromDeckGeneric(base.Owner, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1), c => c.Rarity == rarity))
            .ToList().First();
        await WarframeModCard.TransformTo(original, card);
        SetEventFinished(L10NLookup("WARFRAMEMOD-HELMINTH.pages.INJECTED.description"));
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Cards;
using WarframeMod.Code.Cards.Event;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Events;

public sealed class RivenMaker : CustomEventModel
{
    public override string? CustomInitialPortraitPath => "riven_maker.png".EventBackgroundPath();

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new GoldVar("PriceOfOneRiven", 50),
        new GoldVar("PriceOfTwoRivens", 100),
        new DamageVar(CurrentHpLoss, ValueProp.Unblockable | ValueProp.Unpowered),
    ];

    private int _cycleCount = 0;

    private int CycleCount
	{
		get
		{
			return _cycleCount;
		}
		set
		{
			AssertMutable();
			_cycleCount = value;
		}
	}

    private int CurrentHpLoss => 1 + CycleCount;

    public override bool IsAllowed(IRunState runState)
	{
		return runState.Players.All(p => p.Gold >= base.DynamicVars["PriceOfOneRiven"].BaseValue);
	}

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<EventOption> list = [];

        if (base.Owner.Gold >= base.DynamicVars["PriceOfOneRiven"].BaseValue)
		{
			list.Add(new EventOption(this, BuyOneRiven, "WARFRAMEMOD-RIVEN_MAKER.pages.INITIAL.options.BUY_ONE_RIVEN", HoverTipFactory.FromCard<Riven>()));
		}
		else
		{
			list.Add(new EventOption(this, null, "WARFRAMEMOD-RIVEN_MAKER.pages.INITIAL.options.BUY_ONE_RIVEN_LOCKED"));
		}

        if (base.Owner.Gold >= base.DynamicVars["PriceOfTwoRivens"].BaseValue)
		{
			list.Add(new EventOption(this, BuyTwoRivens, "WARFRAMEMOD-RIVEN_MAKER.pages.INITIAL.options.BUY_TWO_RIVENS", HoverTipFactory.FromCard<Riven>()));
		}
		else
		{
			list.Add(new EventOption(this, null, "WARFRAMEMOD-RIVEN_MAKER.pages.INITIAL.options.BUY_TWO_RIVENS_LOCKED"));
		}

        return list;
    }

    private List<CardHoverTip> GetOldRivenHoverTips()
    {
        List<CardHoverTip> hoverTips = [];
        var cards = base.Owner.Deck.Cards
            .Where(c => c.GetType() == typeof(Riven))
            .ToList();
        foreach (CardModel card in cards)
        {
            hoverTips.Add(new CardHoverTip(card));
        }
        return hoverTips;
    }

    private List<CardHoverTip> GetNewRivenHoverTips(List<Riven> cards)
    {
        List<CardHoverTip> hoverTips = [];
        foreach (CardModel card in cards)
        {
            hoverTips.Add(new CardHoverTip(card));
        }
        return hoverTips;
    }

    private Riven CreateRiven()
    {
        Riven riven = base.Owner.RunState.CreateCard<Riven>(base.Owner);
        riven.Cycle(base.Rng);
        return riven;
    }

    private void GenerateFirstCycleOptions()
    {
        SetEventState(L10NLookup("WARFRAMEMOD-RIVEN_MAKER.pages.IF_CYCLE.description"), [
            new EventOption(this, Cycle, "WARFRAMEMOD-RIVEN_MAKER.pages.ALL.options.CYCLE").ThatDoesDamage(base.DynamicVars.Damage.BaseValue),
            new EventOption(this, StopHere, "WARFRAMEMOD-RIVEN_MAKER.pages.ALL.options.STOP_HERE")
        ]);
    }

    private void GenerateCycleOptions()
    {
        SetEventState(L10NLookup("WARFRAMEMOD-RIVEN_MAKER.pages.CYCLED.description"), [
            new EventOption(this, Cycle, "WARFRAMEMOD-RIVEN_MAKER.pages.ALL.options.CYCLE").ThatDoesDamage(base.DynamicVars.Damage.BaseValue),
            new EventOption(this, StopHere, "WARFRAMEMOD-RIVEN_MAKER.pages.ALL.options.STOP_HERE")
        ]);
    }

    private void GenerateKeepOptions()
    {
        int rivenCount = base.Owner.Deck.Cards
            .Where(c => c.GetType() == typeof(Riven))
            .Count();
        List<Riven> newRivens = [];
        for (int i = 0; i < rivenCount; i++)
        {
            newRivens.Add(CreateRiven());
        }

        SetEventState(L10NLookup("WARFRAMEMOD-RIVEN_MAKER.pages.KEEP.description"), [
            new EventOption(this, () => KeepNewVersion(newRivens), "WARFRAMEMOD-RIVEN_MAKER.pages.ALL.options.KEEP_NEW_VERSION", GetNewRivenHoverTips(newRivens)),
            new EventOption(this, KeepOldVersion, "WARFRAMEMOD-RIVEN_MAKER.pages.ALL.options.KEEP_OLD_VERSION", GetOldRivenHoverTips())
        ]);
    }

    private async Task BuyOneRiven()
    {
        await PlayerCmd.LoseGold(base.DynamicVars["PriceOfOneRiven"].BaseValue, base.Owner, GoldLossType.Spent);
        CardModel card = CreateRiven();
		CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck), 2f);
        GenerateFirstCycleOptions();
    }

    private async Task BuyTwoRivens()
    {
        await PlayerCmd.LoseGold(base.DynamicVars["PriceOfTwoRivens"].BaseValue, base.Owner, GoldLossType.Spent);
        List<CardModel> cards = [CreateRiven(), CreateRiven()];
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(cards, PileType.Deck), 2f);
        GenerateFirstCycleOptions();
    }

    private async Task Cycle()
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner.Creature, CurrentHpLoss, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        CycleCount++;
        base.DynamicVars.Damage.BaseValue = CurrentHpLoss;
        GenerateKeepOptions();
    }

    private Task StopHere()
    {
        SetEventFinished(L10NLookup("WARFRAMEMOD-RIVEN_MAKER.pages.STOP_HERE.description"));
        return Task.CompletedTask;
    }

    private Task KeepOldVersion()
    {
        GenerateCycleOptions();
        return Task.CompletedTask;
    }

    private async Task KeepNewVersion(List<Riven> rivens)
    {
        var cards = base.Owner.Deck.Cards
            .Where(c => c.GetType() == typeof(Riven))
            .ToList();
        for (int i = 0; i < cards.Count; i++)
        {
            await WarframeModCard.TransformTo(cards[i], rivens[i]);
        }
        GenerateCycleOptions();
    }
}
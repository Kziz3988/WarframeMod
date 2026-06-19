
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using WarframeMod.Code.Cards.Event;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Events;

public sealed class SanctuaryOfSimaris : CustomEventModel
{
    public override string? CustomInitialPortraitPath => "sanctuary_of_simaris.png".EventBackgroundPath();

    public override ActModel[] Acts => [ModelDb.Act<Overgrowth>(), ModelDb.Act<Underdocks>()];

    private List<SynthesisScanner> GetScanner(int count, bool isUpgraded = false)
    {
        List<SynthesisScanner> cards = [];
        for (int i = 0; i < count; i++)
        {
            SynthesisScanner card = base.Owner.RunState.CreateCard<SynthesisScanner>(base.Owner);
            if (isUpgraded)
            {
                CardCmd.Upgrade(card);
            }
            cards.Add(card);
        }
        return cards;
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<EventOption> list = [];
		list.Add(new EventOption(this, AFewTargets, "WARFRAMEMOD-SANCTUARY_OF_SIMARIS.pages.INITIAL.options.A_FEW_TARGETS", HoverTipFactory.FromCard<SynthesisScanner>()));
		list.Add(new EventOption(this, LotsOfTargets, "WARFRAMEMOD-SANCTUARY_OF_SIMARIS.pages.INITIAL.options.LOTS_OF_TARGETS", HoverTipFactory.FromCard<SynthesisScanner>(true)));
        return list;
    }

    private async Task AFewTargets()
    {
        List<SynthesisScanner> cards = GetScanner(1);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(cards, PileType.Deck), 2f);
        SetEventFinished(L10NLookup("WARFRAMEMOD-SANCTUARY_OF_SIMARIS.pages.START_HUNTING.description"));
    }

    private async Task LotsOfTargets()
    {
        List<SynthesisScanner> cards = GetScanner(2, true);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(cards, PileType.Deck), 2f);
        SetEventFinished(L10NLookup("WARFRAMEMOD-SANCTUARY_OF_SIMARIS.pages.START_HUNTING.description"));
    }
}
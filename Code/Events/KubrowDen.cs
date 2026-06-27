
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Runs;
using WarframeMod.Code.Cards.Quest;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Relics.Event;

namespace WarframeMod.Code.Events;

public sealed class KubrowDen : CustomEventModel
{
    public override string? CustomInitialPortraitPath => "kubrow_den.png".EventBackgroundPath();

    public override ActModel[] Acts => [ModelDb.Act<Hive>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(20m)];

    public override bool IsAllowed(IRunState runState)
	{
		return runState.Players.All(p => !p.HasEventPet() && !p.Deck.Cards.Any(c => c is ByrdonisEgg) && !p.Relics.Any(r => r.GetType() == typeof(Byrdpip) || r.GetType() == typeof(KubrowRelic)));
	}

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<EventOption> list = [];
		list.Add(new EventOption(this, Eat, "WARFRAMEMOD-KUBROW_DEN.pages.INITIAL.options.EAT"));
		list.Add(new EventOption(this, Take, "WARFRAMEMOD-KUBROW_DEN.pages.INITIAL.options.TAKE", HoverTipFactory.FromCard<KubrowEgg>()));
        return list;
    }

    private async Task Eat()
	{
		await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars.Heal.BaseValue);
		SetEventFinished(L10NLookup("WARFRAMEMOD-KUBROW_DEN.pages.EAT.description"));
	}

	private async Task Take()
	{
		CardModel card = base.Owner.RunState.CreateCard<KubrowEgg>(base.Owner);
		CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck), 2f);
		SetEventFinished(L10NLookup("WARFRAMEMOD-KUBROW_DEN.pages.TAKE.description"));
	}
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using WarframeMod.Code.Cards;
using WarframeMod.Code.Cards.Event;
using WarframeMod.Code.Encounters;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Events;

public sealed class DeathMarkOfCorpus : CustomEventModel
{
    public override string? CustomInitialPortraitPath => "death_mark_of_grineer.png".EventBackgroundPath();

    public override bool IsShared => true;

    public override ActModel[] Acts => [ModelDb.Act<Hive>(), ModelDb.Act<Glory>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    public override bool IsAllowed(IRunState runState)
    {
        return runState.Players.All(p => p.Creature.CurrentHp >= 50);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<EventOption> list = [];
		list.Add(new EventOption(this, Fight, "WARFRAMEMOD-DEATH_MARK_OF_CORPUS.pages.INITIAL.options.FIGHT"));
		list.Add(new EventOption(this, Run, "WARFRAMEMOD-DEATH_MARK_OF_CORPUS.pages.INITIAL.options.RUN", HoverTipFactory.FromCard<Unarmed>()));
        return list;
    }

    private Task Fight()
    {
		List<Reward> list = new List<Reward>();
		foreach (Player player in base.Owner.RunState.Players)
		{
			list.Add(new RelicReward(RelicRarity.Rare, player));
		}
		EnterCombatWithoutExitingEvent<ZanukaHunterEvent>(list, shouldResumeAfterCombat: false);
		return Task.CompletedTask;
    }

    private async Task Run()
    {
        IEnumerable<CardModel> cards = PileType.Deck.GetPile(base.Owner).Cards.Where(c => c.IsTransformable).ToList().StableShuffle(base.Rng)
			.Take(base.DynamicVars.Cards.IntValue);
        foreach(CardModel card in cards)
        {
            CardModel unarmed = base.Owner.RunState.CreateCard<Unarmed>(base.Owner);
            await WarframeModCard.TransformTo(card, unarmed);
        }
        SetEventFinished(L10NLookup("WARFRAMEMOD-DEATH_MARK_OF_CORPUS.pages.RUN_AWAY.description"));
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Event;

[Pool(typeof(EventCardPool))]
public class SynthesisScanner() : WarframeModCard(1, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Fatal)
    ];
	protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(1m, ValueProp.Move)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
		CardKeyword.Eternal,
		CardKeyword.Retain
	];

    private bool _isTriggered = false;

    [SavedProperty]
    public bool IsTriggered
	{
		get
		{
			return _isTriggered;
		}
		set
		{
			AssertMutable();
			_isTriggered = value;
		}
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		AbstractRoom currentRoom = base.CombatState.RunState.CurrentRoom;
		if (currentRoom is CombatRoom combatRoom)
		{
			ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
			bool shouldTriggerFatal = cardPlay.Target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
			AttackCommand attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
				.WithHitFx("vfx/vfx_attack_slash")
				.Execute(choiceContext);
			if (shouldTriggerFatal && attackCommand.Results.SelectMany((List<DamageResult> r) => r).Any((DamageResult r) => r.WasTargetKilled))
			{
                IsTriggered = true;
				combatRoom.AddExtraReward(base.Owner, new RelicReward(base.Owner));
				await PowerCmd.Apply<RewardOfSimarisPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
			}
		}
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (IsTriggered && base.DeckVersion != null)
        {
            await CardPileCmd.RemoveFromDeck(base.DeckVersion);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}

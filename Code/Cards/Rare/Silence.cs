using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace WarframeMod.Code.Cards.Rare;

public class Silence() : WarframeModCard(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        StunIntent.GetStaticHoverTip()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DynamicVar("Decrement", 1)
	];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		Creature target = cardPlay.Target;
        await CreatureCmd.Stun(target);
		var powersToDecrement = target.Powers
			.Where(p => p.StackType == PowerStackType.Counter && p.Type == PowerType.Buff)
			.ToList();

		var powersToIncrement = target.Powers
			.Where(p => p.GetType() == typeof(HardenedShellPower) || p.StackType == PowerStackType.Counter && p.Type == PowerType.Debuff && p.Amount < 0)
			.ToList();
		
		foreach (PowerModel power in powersToDecrement)
		{
			await PowerCmd.ModifyAmount(power, -base.DynamicVars["Decrement"].BaseValue, target, this);
		}
		foreach (PowerModel power in powersToIncrement)
		{
			await PowerCmd.ModifyAmount(power, base.DynamicVars["Decrement"].BaseValue, target, this);
		}
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
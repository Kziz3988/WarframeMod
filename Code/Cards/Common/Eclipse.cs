using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace WarframeMod.Code.Cards.Common;

public class Eclipse() : WarframeModCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromPower<WeakPower>(),
		HoverTipFactory.FromPower<VulnerablePower>()
	];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
		new PowerVar<WeakPower>(2),
		new PowerVar<VulnerablePower>(1)
	];

	private static bool IntendsToDefend(MonsterModel monster)
	{
		return monster.NextMove.Intents.Any(delegate(AbstractIntent intent)
		{
			IntentType intentType = intent.IntentType;
			return intentType == IntentType.Defend;
		});
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		if (cardPlay.Target.Monster.IntendsToAttack)
		{
			await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
		}
		if (IntendsToDefend(cardPlay.Target.Monster))
		{
			await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
		}
	}

    protected override void OnUpgrade()
    {
		base.DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}
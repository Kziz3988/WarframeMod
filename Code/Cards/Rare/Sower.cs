using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Cards.Rare;

public class Sower() : WarframeModCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("DamagePercent", 10m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
	
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        DamageVar damage = new(cardPlay.Target.CurrentHp * (base.DynamicVars["DamagePercent"].BaseValue / 100m), ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move);
		await CreatureCmd.Damage(choiceContext, cardPlay.Target, damage, this);
	}

	protected override void OnUpgrade()
	{
		RemoveKeyword(CardKeyword.Exhaust);
	}
}
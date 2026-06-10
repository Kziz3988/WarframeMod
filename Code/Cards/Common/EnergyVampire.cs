using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Common;

public class EnergyVampire() : WarframeModCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    public override TargetType TargetType
	{
		get
		{
			if (!IsUpgraded)
			{
				return TargetType.AnyEnemy;
			}
			return TargetType.AllEnemies;
		}
	}
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(3)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        if (base.IsUpgraded)
        {
            await PowerCmd.Apply<EnergyVampirePower>(base.CombatState.HittableEnemies, base.DynamicVars.Energy.BaseValue, base.Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<EnergyVampirePower>(cardPlay.Target, base.DynamicVars.Energy.BaseValue, base.Owner.Creature, this);
        }
	}
}
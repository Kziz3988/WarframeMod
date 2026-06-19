using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Common;

public class EnergyVampire() : WarframeModCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
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
    protected override IEnumerable<DynamicVar> CanonicalVars => [
		new EnergyVar(1),
		new DamageVar(5m, ValueProp.Move)
	];

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (base.IsUpgraded)
        {
            await PowerCmd.Apply<EnergyVampirePower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Energy.BaseValue, base.Owner.Creature, this);
        }
        else
        {
			ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            await PowerCmd.Apply<EnergyVampirePower>(choiceContext, cardPlay.Target, base.DynamicVars.Energy.BaseValue, base.Owner.Creature, this);
        }

		foreach (Creature enemy in base.CombatState.HittableEnemies)
		{
			if (enemy.GetPower<EnergyVampirePower>() != null)
			{
				await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(enemy)
					.WithHitFx("vfx/vfx_attack_slash")
					.Execute(choiceContext);
			}
		}
	}

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
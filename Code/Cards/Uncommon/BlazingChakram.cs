using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Uncommon;

public class BlazingChakram() : WarframeModCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
	protected override HashSet<CardTag> CanonicalTags => [(CardTag)WarframeModCardTag.Element];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromPower<HeatPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block)
	];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(16m, ValueProp.Move),
		new PowerVar<HeatPower>(3m),
		new EnergyVar(1)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		bool flag = false;
		foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
		{
			if ((await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(hittableEnemy)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext)).Results.Any((DamageResult r) => r.WasTargetKilled))
			{
				if (!flag) {
					await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, base.Owner);
				}
				if (!base.IsUpgraded)
				{
					flag = true;
				}
			}
			await PowerCmd.Apply<HeatPower>(hittableEnemy, base.DynamicVars["HeatPower"].BaseValue, base.Owner.Creature, this);
		}

    }

    protected override void OnUpgrade()
    {
		base.DynamicVars.Damage.UpgradeValueBy(4m);
        base.DynamicVars["HeatPower"].UpgradeValueBy(1m);
    }
}

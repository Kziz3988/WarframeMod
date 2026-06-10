using System.Collections.Generic;
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

public class Pyrotechnics() : WarframeModCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
{
	protected override HashSet<CardTag> CanonicalTags => [(CardTag)WarframeModCardTag.Element];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<HeatPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block)
	];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(3m, ValueProp.Move),
        new PowerVar<HeatPower>(1m),
        new RepeatVar(3)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for (int i = 0; i < base.DynamicVars.Repeat.IntValue; i++)
		{
			Creature enemy = base.Owner.RunState.Rng.CombatTargets.NextItem(base.CombatState.HittableEnemies);
			if (enemy == null)
			{
				continue;
			}
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(enemy)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
			await PowerCmd.Apply<HeatPower>(enemy, base.DynamicVars["HeatPower"].BaseValue, base.Owner.Creature, this);
		}
    }

    protected override void OnUpgrade()
    {
		base.DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}

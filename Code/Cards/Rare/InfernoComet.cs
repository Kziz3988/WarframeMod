using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Rare;

public class InfernoComet() : WarframeModCard(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
	protected override HashSet<CardTag> CanonicalTags => [(CardTag)WarframeModCardTag.Element];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<HeatPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(6m, ValueProp.Move),
        new PowerVar<HeatPower>(3)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this)
			.TargetingAllOpponents(base.CombatState)
			.WithHitFx("vfx/vfx_giant_horizontal_slash")
			.Execute(choiceContext);
        int enemyCount = base.CombatState.HittableEnemies.Count;
        await PowerCmd.Apply<HeatPower>(base.CombatState.HittableEnemies, base.DynamicVars["HeatPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<InfernoCometPower>(base.Owner.Creature, enemyCount, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
		base.DynamicVars.Damage.UpgradeValueBy(2m);
        base.DynamicVars["HeatPower"].UpgradeValueBy(1m);
    }
}

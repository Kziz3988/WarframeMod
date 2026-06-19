using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Uncommon;

public class Arcsphere() : WarframeModCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
	protected override HashSet<CardTag> CanonicalTags => [(CardTag)WarframeModCardTag.Element];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ElectricityPower>(),
        StunIntent.GetStaticHoverTip()
    ];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(6m, ValueProp.Move),
		new PowerVar<ElectricityPower>(2m)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int enemyCount = base.CombatState.HittableEnemies.Count;
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this)
			.TargetingAllOpponents(base.CombatState)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
		await PowerCmd.Apply<ElectricityPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars["ElectricityPower"].BaseValue, base.Owner.Creature, this);

        if (enemyCount >= 3)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
		base.DynamicVars.Damage.UpgradeValueBy(2m);
        base.DynamicVars["ElectricityPower"].UpgradeValueBy(1m);
    }
}

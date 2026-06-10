using System;
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

namespace WarframeMod.Code.Cards.Common;

public class Fireball() : WarframeModCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{	
	protected override HashSet<CardTag> CanonicalTags => [(CardTag)WarframeModCardTag.Element];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<HeatPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];
	protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar("DirectDamage", 6m, ValueProp.Move),
        new DamageVar("SplashDamage", 2m, ValueProp.Move),
		new PowerVar<HeatPower>("DirectHeatPower", 5m),
        new PowerVar<HeatPower>("SplashHeatPower", 1m),
        new PowerVar<HeatPower>("HeatPower", 5m) // For Kumihimo judgement
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
		{
            if (hittableEnemy == cardPlay.Target)
            {
                await DamageCmd.Attack(base.DynamicVars["DirectDamage"].BaseValue).FromCard(this).Targeting(hittableEnemy)
			    .WithHitFx("vfx/vfx_attack_slash")
			    .Execute(choiceContext);
			    await PowerCmd.Apply<HeatPower>(hittableEnemy, base.DynamicVars["DirectHeatPower"].BaseValue, base.Owner.Creature, this);
            }
            else{
                await DamageCmd.Attack(base.DynamicVars["SplashDamage"].BaseValue).FromCard(this).Targeting(hittableEnemy)
			    .WithHitFx("vfx/vfx_attack_slash")
			    .Execute(choiceContext);
			    await PowerCmd.Apply<HeatPower>(hittableEnemy, base.DynamicVars["SplashHeatPower"].BaseValue, base.Owner.Creature, this);
            }
		}
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["DirectDamage"].UpgradeValueBy(1m);
        base.DynamicVars["SplashDamage"].UpgradeValueBy(1m);
        base.DynamicVars["DirectHeatPower"].UpgradeValueBy(1m);
        base.DynamicVars["SplashHeatPower"].UpgradeValueBy(1m);
        base.DynamicVars["HeatPower"].UpgradeValueBy(1m);
    }
}

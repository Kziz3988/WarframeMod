using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Basic;

public class ExaltedBlade() : WarframeModCard(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{	
	protected override bool HasEnergyCostX => true;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SlashPower>()];
	protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
		new PowerVar<SlashPower>("SlashPower", 1m)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        int num = ResolveEnergyXValue();
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(num).FromCard(this)
			.Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_giant_horizontal_slash")
			.Execute(choiceContext);
		await PowerCmd.Apply<SlashPower>(cardPlay.Target, base.DynamicVars["SlashPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
		base.DynamicVars["SlashPower"].UpgradeValueBy(1m);
    }
}

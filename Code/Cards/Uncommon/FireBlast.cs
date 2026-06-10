using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Uncommon;

public class FireBlast() : WarframeModCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
	protected override HashSet<CardTag> CanonicalTags => [(CardTag)WarframeModCardTag.Element];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<HeatPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block),
		HoverTipFactory.FromPower<FrailPower>()
	];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(10m, ValueProp.Move),
        new PowerVar<HeatPower>(2m),
		new PowerVar<FrailPower>(1m)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await PowerCmd.Apply<HeatPower>(cardPlay.Target, base.DynamicVars["HeatPower"].BaseValue, base.Owner.Creature, this);
		await PowerCmd.Apply<FrailPower>(cardPlay.Target, base.DynamicVars["FrailPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
		base.DynamicVars.Damage.UpgradeValueBy(2m);
        base.DynamicVars["HeatPower"].UpgradeValueBy(1m);
        base.DynamicVars["FrailPower"].UpgradeValueBy(1m);
    }
}

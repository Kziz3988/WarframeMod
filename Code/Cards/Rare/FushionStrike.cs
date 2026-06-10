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

namespace WarframeMod.Code.Cards.Rare;

public class FusionStrike() : WarframeModCard(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
	protected override HashSet<CardTag> CanonicalTags => [
		CardTag.Strike
	];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromPower<FrailPower>(),
		HoverTipFactory.FromPower<TauPower>()
	];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(4m, ValueProp.Move),
		new RepeatVar(3),
		new PowerVar<FrailPower>(3),
		new PowerVar<TauPower>(1)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(base.DynamicVars.Repeat.IntValue).FromCard(this)
			.TargetingAllOpponents(base.CombatState)
			.WithHitFx("vfx/vfx_giant_horizontal_slash")
			.Execute(choiceContext);
		await PowerCmd.Apply<FrailPower>(base.CombatState.HittableEnemies, base.DynamicVars["FrailPower"].BaseValue, base.Owner.Creature, this);
		await PowerCmd.Apply<TauPower>(base.CombatState.HittableEnemies, base.DynamicVars["TauPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
		base.DynamicVars.Damage.UpgradeValueBy(1m);
        base.DynamicVars["FrailPower"].UpgradeValueBy(1m);
		base.DynamicVars["TauPower"].UpgradeValueBy(1m);
    }
}

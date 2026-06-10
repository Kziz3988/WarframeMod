using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Rare;

public class Avalanche() : WarframeModCard(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
	protected override HashSet<CardTag> CanonicalTags => [(CardTag)WarframeModCardTag.Element];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ColdPower>(),
        StunIntent.GetStaticHoverTip(),
		HoverTipFactory.FromPower<FrailPower>()
	];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(13m, ValueProp.Move),
        new PowerVar<ColdPower>(7),
		new PowerVar<FrailPower>(3)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this)
			.TargetingAllOpponents(base.CombatState)
			.WithHitFx("vfx/vfx_giant_horizontal_slash")
			.Execute(choiceContext);
        await PowerCmd.Apply<ColdPower>(base.CombatState.HittableEnemies, base.DynamicVars["ColdPower"].BaseValue, base.Owner.Creature, this);
		await PowerCmd.Apply<FrailPower>(base.CombatState.HittableEnemies, base.DynamicVars["FrailPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
		base.DynamicVars.Damage.UpgradeValueBy(3m);
        base.DynamicVars["ColdPower"].UpgradeValueBy(2m);
        base.DynamicVars["FrailPower"].UpgradeValueBy(1m);
    }
}

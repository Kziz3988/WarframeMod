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

namespace WarframeMod.Code.Cards.Ancient;

public class ChromaticBlade() : WarframeModCard(0, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
{	
	protected override bool HasEnergyCostX => true;
	protected override HashSet<CardTag> CanonicalTags => [(CardTag)WarframeModCardTag.Element];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<SlashPower>(),
        HoverTipFactory.FromPower<ElectricityPower>(),
        StunIntent.GetStaticHoverTip()
    ];
	protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
		new PowerVar<SlashPower>("SlashPower", 3m),
        new PowerVar<ElectricityPower>("ElectricityPower", 2m)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		int num = ResolveEnergyXValue();
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(num).FromCard(this)
			.TargetingAllOpponents(base.CombatState)
			.WithHitFx("vfx/vfx_giant_horizontal_slash")
			.Execute(choiceContext);
        await PowerCmd.Apply<SlashPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars["SlashPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<ElectricityPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars["ElectricityPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
        base.DynamicVars["SlashPower"].UpgradeValueBy(2m);
        base.DynamicVars["ElectricityPower"].UpgradeValueBy(1m);
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Character;
using WarframeMod.Code.Powers.Buff;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Ancient;

[Pool(typeof(VoltCardPool))]
public class Capacitance() : WarframeModCard(2, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
{
	protected override HashSet<CardTag> CanonicalTags => [(CardTag)WarframeModCardTag.Element];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ElectricityPower>(),
        StunIntent.GetStaticHoverTip(),
        HoverTipFactory.FromPower<ShieldPower>()
    ];
	protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new PowerVar<ElectricityPower>("ElectricityPower", 2m),
        new RepeatVar(2)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		AttackCommand attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(base.DynamicVars.Repeat.IntValue).FromCard(this, cardPlay)
			.TargetingAllOpponents(base.CombatState)
			.WithHitFx("vfx/vfx_giant_horizontal_slash")
			.Execute(choiceContext);
        await PowerCmd.Apply<ElectricityPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Repeat.IntValue * base.DynamicVars["ElectricityPower"].BaseValue, base.Owner.Creature, this);
        await ShieldPower.ApplyShield(base.Owner.Creature, attackCommand.Results.SelectMany((List<DamageResult> r) => r).Sum((DamageResult r) => r.TotalDamage + r.OverkillDamage) / 2, 0, 0, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
        base.DynamicVars["ElectricityPower"].UpgradeValueBy(1m);
    }
}

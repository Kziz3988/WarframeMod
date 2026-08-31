using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Character;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Basic;

[Pool(typeof(VoltCardPool))]
public class Discharge() : WarframeModCard(2, CardType.Attack, CardRarity.Basic, TargetType.RandomEnemy)
{
	protected override HashSet<CardTag> CanonicalTags => [(CardTag)WarframeModCardTag.Element];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ElectricityPower>(),
        StunIntent.GetStaticHoverTip()
    ];
	protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
		new PowerVar<ElectricityPower>(1m),
        new RepeatVar(2)
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
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(enemy)
                .WithHitFx("vfx/vfx_giant_horizontal_slash")
                .Execute(choiceContext);
            await PowerCmd.Apply<ElectricityPower>(choiceContext, enemy, base.DynamicVars["ElectricityPower"].BaseValue, base.Owner.Creature, this);
		}
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}

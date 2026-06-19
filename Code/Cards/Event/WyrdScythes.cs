using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Cards.Event;

[Pool(typeof(EventCardPool))]
public class WyrdScythes() : WarframeModCard(2, CardType.Attack, CardRarity.Event, TargetType.RandomEnemy)
{
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<WarframeModCardPool>();
    
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<VulnerablePower>()
	];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(4m, ValueProp.Move),
        new RepeatVar(2),
        new PowerVar<VulnerablePower>(1m)
	];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
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
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(enemy)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
		}
    }

    protected override void OnUpgrade()
    {
		base.DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}

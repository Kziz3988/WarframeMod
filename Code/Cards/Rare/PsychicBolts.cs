using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Rare;

public class PsychicBolts() : WarframeModCard(2, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<ShieldPower>()
	];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(3m, ValueProp.Move),
        new RepeatVar(4),
        new DynamicVar("TotalShield", 1m)
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
            await CreatureCmd.LoseBlock(enemy, base.DynamicVars["TotalShield"].BaseValue);
			await ShieldPower.ApplyShield(base.Owner.Creature, base.DynamicVars["TotalShield"].IntValue, 0, 0, base.Owner.Creature, this);
		}
    }

    protected override void OnUpgrade()
    {
		base.DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}

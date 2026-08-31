using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Cards.Common;

public class Whipclaw() : WarframeModCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(2m, ValueProp.Move),
		new RepeatVar(3),
        new DynamicVar("Increment", 1m)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal tempDamage = base.DynamicVars.Damage.BaseValue;
		for (int i = 0; i < base.DynamicVars.Repeat.BaseValue; i++)
        {
            foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
            {
                await DamageCmd.Attack(tempDamage).FromCard(this, cardPlay).Targeting(hittableEnemy)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
                tempDamage += base.DynamicVars["Increment"].BaseValue;
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Cards.Uncommon;

public class TempestBarrage() : WarframeModCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(1m, ValueProp.Move),
		new RepeatVar(5),
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(base.DynamicVars.Repeat.IntValue).FromCard(this)
			.TargetingAllOpponents(base.CombatState)
			.WithHitFx("vfx/vfx_giant_horizontal_slash")
			.Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Repeat.UpgradeValueBy(2m);
    }
}

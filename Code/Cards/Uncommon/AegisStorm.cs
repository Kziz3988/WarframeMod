using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Uncommon;

public class AegisStorm() : WarframeModCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<NewSoarPower>(),
        HoverTipFactory.FromPower<ShieldPower>()
	];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<NewSoarPower>(1m),
        new CalculationBaseVar(0m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => card.Owner.Creature.GetPower<ShieldPower>()?.TotalShield ?? 0)
	];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<NewSoarPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars["NewSoarPower"].BaseValue, base.Owner.Creature, this);
        ShieldPower? shield = base.Owner.Creature.GetPower<ShieldPower>();
        if (shield == null)
        {
            return;
        }
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage).FromCard(this)
			.TargetingAllOpponents(base.CombatState)
			.WithHitFx("vfx/vfx_giant_horizontal_slash")
			.Execute(choiceContext);
        await ShieldPower.ApplyShield(base.Owner.Creature, -shield.TotalShield, 0, 0, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Relics.Shop;

public class ArcaneOvercharge : WarframeModRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ShieldPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6m, ValueProp.Unpowered | ValueProp.SkipHurtAnim)
    ];

    private ShieldPower? shield = null;

    private void Initialize(ShieldPower? shieldPower)
    {
        if (shieldPower == null)
        {
            return;
        }
        shield = shieldPower;
        shield.OnShieldChanged += AfterShieldChanged;
    }

    public override Task AfterObtained()
    {
        ShieldPower? shieldPower = base.Owner.Creature.GetPower<ShieldPower>();
        Initialize(shieldPower);
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (shield == null && power.Owner == base.Owner.Creature && power.GetType() == typeof(ShieldPower))
        {
            Initialize(power as ShieldPower);
        }
        return Task.CompletedTask;        
    }

    private async void AfterShieldChanged(ShieldPower power, ShieldData old, ShieldData current)
    {
        if (shield == null)
        {
            return;
        }
        if (current.TotalShield > old.TotalShield && current.OverShield > 0)
        {
            Flash();
            Creature? creature = base.Owner.RunState.Rng.CombatTargets.NextItem(base.Owner.Creature.CombatState.HittableEnemies);
            if (creature != null)
            {
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), creature, base.DynamicVars.Damage, base.Owner.Creature);
            }
        }
    }

    public override Task AfterCombatEnd(CombatRoom _)
	{
		base.Status = RelicStatus.Normal;
        if (shield != null)
        {
            shield.OnShieldChanged -= AfterShieldChanged;
        }
        shield = null;
		return Task.CompletedTask;
	}

    public override Task AfterRemoved()
	{
        if (shield != null)
        {
            shield.OnShieldChanged -= AfterShieldChanged;
        }
		return Task.CompletedTask;
	}
}
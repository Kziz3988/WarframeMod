using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Buff;

public sealed class ShatterShieldPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	private ShieldPower? shield = null;

    private void Initialize(ShieldPower? shieldPower)
    {
        if (shieldPower == null)
        {
            return;
        }
        shield = shieldPower;
        shield.OnShieldDamaged += AfterShieldDamaged;
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        ShieldPower? shieldPower = base.Owner.GetPower<ShieldPower>();
        if (shieldPower != null)
        {
            Initialize(shieldPower);
        }
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (shield == null && power.Owner == base.Owner && power.GetType() == typeof(ShieldPower))
        {
            Initialize(power as ShieldPower);
        }
        return Task.CompletedTask;
    }

    private async void AfterShieldDamaged(ShieldPower power, int amount, Creature? dealer, CardModel? cardSource)
    {
        if (power.Owner != base.Owner || amount <= 0 || dealer == null)
        {
            return;
        }
        decimal multiplier = base.Amount / 100m;
        decimal reflectAmount = amount * multiplier;
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), dealer, reflectAmount, ValueProp.Unpowered, base.Owner, null);
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Side)
		{
            if (shield != null)
            {
                shield.OnShieldDamaged -= AfterShieldDamaged;
            }
            await PowerCmd.Remove(this);
		}
    }
}

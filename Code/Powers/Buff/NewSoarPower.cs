using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Buff;

public sealed class NewSoarPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

	public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target != base.Owner)
		{
			return 1m;
		}
		if (!props.IsPoweredAttack())
		{
			return 1m;
		}
		return 0.5m;
	}

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != base.Owner.Side)
		{
			return;
		}
		if (base.Owner.IsAlive)
		{
			await PowerCmd.Decrement(this);
		}
		else
		{
			await Cmd.CustomScaledWait(0.1f, 0.25f);
		}
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace WarframeMod.Code.Powers.Debuff;

public partial class TauPower : WarframeModPower
{
	public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

	private List<PowerModel>? _doubledPowers;

	private List<PowerModel> DoubledPowers
	{
		get
		{
			AssertMutable();
			if (_doubledPowers == null)
			{
				_doubledPowers = new List<PowerModel>();
			}
			return _doubledPowers;
		}
	}

	public override Task BeforeCombatStart()
	{
		DoubledPowers.Clear();
		return Task.CompletedTask;
	}

	public override Task BeforePowerAmountChanged(PowerModel power, decimal amount, Creature target, Creature? applier, CardModel? cardSource)
	{
		if (target != base.Owner)
		{
			return Task.CompletedTask;
		}
		if (!power.IsVisible)
		{
			return Task.CompletedTask;
		}
		if (power.GetTypeForAmount(amount) != PowerType.Debuff)
		{
			return Task.CompletedTask;
		}
		DoubledPowers.Add(power);
		return Task.CompletedTask;
	}

    public override decimal ModifyPowerAmountGivenMultiplicative(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
		if(target != base.Owner)
		{
			return 1m;
		}
		if (HasDoubledTemporaryPowerSource(power))
		{
			return 1m;
		}
		if (power.GetTypeForAmount(amount) != PowerType.Debuff)
		{
			return 1m;
		}
		return 2m;        
    }

	private bool HasDoubledTemporaryPowerSource(PowerModel power)
	{
		return DoubledPowers.OfType<ITemporaryPower>().Any(p => p.InternallyAppliedPower.GetType() == power.GetType());
	}

	public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
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

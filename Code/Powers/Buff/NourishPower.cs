using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace WarframeMod.Code.Powers.Buff;

public sealed class NourishPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyEnergyGain(Player player, decimal amount)
    {
        if (player != base.Owner.Player)
		{
			return amount;
		}
        return amount + base.Amount;
    }
}

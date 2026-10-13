using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace WarframeMod.Code.Powers.Buff;

public sealed class NourishPower : WarframeModPower
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy(this)
    ];

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

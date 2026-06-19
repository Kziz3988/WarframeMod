using MegaCrit.Sts2.Core.Entities.Powers;

namespace WarframeMod.Code.Powers.Buff;

public sealed class RewardOfSimarisPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;
}

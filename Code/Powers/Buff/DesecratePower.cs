using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace WarframeMod.Code.Powers.Buff;

public sealed class DesecratePower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		GetExtraHoverTip()
	];

    private int ExtraAward { get; set; }

    protected override HoverTip GetExtraHoverTip()
	{
		StringBuilder stringBuilder = new();
		LocString locString = ExtraDescription;
        locString.Add("Amount", base.Amount);
		locString.Add("ExtraAward", ExtraAward);
		DynamicVars.AddTo(locString);
		stringBuilder.Append(locString.GetFormattedText());
		return new HoverTip(this, stringBuilder.ToString(), true);
	}

    public override Task BeforeDeath(Creature creature)
    {
        if (creature.Side == base.Owner.Side)
        {
        	return Task.CompletedTask;
        }
		bool shouldTriggerFatal = creature.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
		if (shouldTriggerFatal)
		{
			ExtraAward += base.Amount;
		}
		return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
	{
		room.AddExtraReward(base.Owner.Player, new GoldReward(ExtraAward, base.Owner.Player));
		return Task.CompletedTask;
	}
}

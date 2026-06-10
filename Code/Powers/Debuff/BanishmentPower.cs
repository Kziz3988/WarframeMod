using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace WarframeMod.Code.Powers.Debuff;

public partial class BanishmentPower : WarframeModPower
{
	public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		StunIntent.GetStaticHoverTip()
	];
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != base.Owner.Side)
		{
			return;
		}
		if (base.Owner.IsAlive)
		{
			await CreatureCmd.Stun(base.Owner);
			await PowerCmd.Decrement(this);
		}
		else
		{
			await Cmd.CustomScaledWait(0.1f, 0.25f);
		}
    }
}

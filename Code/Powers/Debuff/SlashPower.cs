using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Debuff;

public partial class SlashPower : WarframeModPower
{
	public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

	public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
	{
		if (side != base.Owner.Side)
		{
			return;
		}
		await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
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

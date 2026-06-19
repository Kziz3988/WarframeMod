using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Powers.Debuff;

public partial class SlashPower : WarframeModPower
{
	public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStartLate(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
		if (side != base.Owner.Side)
		{
			return;
		}

		ShieldPower? shield = base.Owner.GetPower<ShieldPower>();

		if (shield == null || shield.TotalShield == 0)
		{
			await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
		}
		else if (shield.TotalShield >= base.Amount)
		{
			shield.DamageShield(base.Amount, null, null);
		}
		else
		{
			await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount - shield.TotalShield, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
			shield.DamageShield(base.Amount, null, null);
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

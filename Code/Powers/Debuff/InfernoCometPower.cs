using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace WarframeMod.Code.Powers.Debuff;

public partial class InfernoCometPower : WarframeModPower
{
	public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == base.Owner.Side)
        {
            ArgumentNullException.ThrowIfNull(base.Owner.Player, "base.Owner.Player");
            await PlayerCmd.LoseEnergy(base.Amount, base.Owner.Player);
            await PowerCmd.Remove(this);
        }        
    }
}

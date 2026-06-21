using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Buff;

public sealed class RiotShieldPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;

    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        if (creature == base.Owner)
        {
            foreach (Creature ally in base.CombatState.GetCreaturesOnSide(base.Owner.Side))
            {
                if (ally != base.Owner)
                {
                    await CreatureCmd.GainBlock(ally, amount, props, null);
                }
            }
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace WarframeMod.Code.Powers.Debuff;

public partial class TransferenceStaticPower : WarframeModPower
{
	public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
    	if (side != base.Owner.Side)
		{
			return;
		}
        Flash();
        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount, false);
    }
}

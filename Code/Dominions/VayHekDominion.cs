using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Dread;
using MegaCrit.Sts2.Core.Rooms;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Dominions;

public class VayHekDominion : DominionModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy(this)
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom combatRoom)
		{
			await PowerCmd.Apply<VayHekDominionPower>(new ThrowingPlayerChoiceContext(), combatRoom.CombatState.Players.Select(p => p.Creature), 5m, null, null);
		}
    }
}
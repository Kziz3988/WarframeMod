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

public class LechKrilDominion : DominionModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<HeatPower>()
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom combatRoom)
		{
			await PowerCmd.Apply<HeatPower>(new ThrowingPlayerChoiceContext(), combatRoom.CombatState.Players.Select(p => p.Creature), 5m, null, null);
		}
    }
}
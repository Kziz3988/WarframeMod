using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Dread;
using MegaCrit.Sts2.Core.Rooms;

namespace WarframeMod.Code.Dominions;

public class PhoridDominion : DominionModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<Dazed>()
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom combatRoom)
		{
			await CardPileCmd.AddToCombatAndPreview<Dazed>(combatRoom.CombatState.Players.Select(p => p.Creature), PileType.Discard, 5, null);
		}
    }
}
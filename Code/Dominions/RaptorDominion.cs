using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Dread;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Dominions;

public class RaptorDominion : DominionModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<NewSoarPower>()
    ];

    protected override Task AfterEnemiesEnteredCombat(IEnumerable<Creature> creatures)
	{
		return PowerCmd.Apply<RaptorDominionPower>(new ThrowingPlayerChoiceContext(), creatures, 1m, null, null);
	}
}
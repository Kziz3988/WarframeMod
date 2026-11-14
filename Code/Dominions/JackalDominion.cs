using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Dread;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Dominions;

public class JackalDominion : DominionModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ElectrifiedPower>()
    ];

    protected override Task AfterEnemiesEnteredCombat(IEnumerable<Creature> creatures)
	{
		return PowerCmd.Apply<ElectrifiedPower>(new ThrowingPlayerChoiceContext(), creatures, 3m, null, null);
	}
}
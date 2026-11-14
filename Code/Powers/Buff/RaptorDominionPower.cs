
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace WarframeMod.Code.Powers.Buff;

public partial class RaptorDominionPower : WarframeModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<NewSoarPower>()
    ];

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
		if (!participants.Contains(base.Owner))
		{
			return;
		}
		PlayerCombatState? playerCombatState = combatState.PlayerCreatures.FirstOrDefault()?.Player?.PlayerCombatState;
        if (playerCombatState != null && playerCombatState.TurnNumber >= 2)
		{
			await PowerCmd.Apply<NewSoarPower>(new ThrowingPlayerChoiceContext(), base.Owner, 1m, null, null);
		}
    }
}
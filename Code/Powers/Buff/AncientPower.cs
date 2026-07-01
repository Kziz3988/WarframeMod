using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Powers.Buff;

public sealed class AncientPower : WarframeModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<OverguardPower>(),
        StunIntent.GetStaticHoverTip()
    ];

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == base.Owner.Side)
        {
            int count = base.CombatState.GetOpponentsOf(base.Owner).Where(c => c?.Player?.Character is Excalibur).Count() - 1;
            if (count > 0)
            {
                await PowerCmd.Apply<OverguardPower>(new ThrowingPlayerChoiceContext(), base.Owner, count, base.Owner, null);                
            }
        }
    }

    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
	{
		return false;
	}
}

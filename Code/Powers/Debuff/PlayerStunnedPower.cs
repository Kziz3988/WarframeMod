using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace WarframeMod.Code.Powers.Debuff;

public sealed class PlayerStunnedPower : WarframeModPower
{
	public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.None;

    private void TryEndTurn()
    {
        if (base.Owner.Player != null)
        {
            PlayerCmd.EndTurn(base.Owner.Player, canBackOut: false);
        }
        else if (base.Owner.Monster != null)
        {
            CreatureCmd.Stun(base.Owner);
        }
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != base.Owner.Side)
        {
            return Task.CompletedTask;
        }
        TryEndTurn();
        return Task.CompletedTask;
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        TryEndTurn();
        return Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

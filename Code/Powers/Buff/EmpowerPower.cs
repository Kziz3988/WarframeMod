using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Buff;

public sealed class EmpowerPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != base.Owner || cardSource == null || cardSource.Type != CardType.Attack || !props.IsPoweredAttack())
        {
            return 0m;
        }
        return base.Amount;
    }

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;
        if (card.Owner == base.Owner.Player && card.Type == CardType.Attack)
        {
            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == base.Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}

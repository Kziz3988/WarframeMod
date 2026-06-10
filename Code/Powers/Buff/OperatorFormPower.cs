using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Powers.Buff;

public sealed class OperatorFormPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<IntangiblePower>(),
        HoverTipFactory.FromPower<TransferenceStaticPower>()
    ];

    private bool isAttackPlayed = false;

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;
        if (card.Owner.Creature == base.Owner && card.Type == CardType.Attack)
        {
            isAttackPlayed = true;
        }
        return Task.CompletedTask;
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == base.Owner.Side && !isAttackPlayed)
        {
            Flash();
            await PowerCmd.Apply<IntangiblePower>(base.Owner, base.Amount, base.Owner, null);
        }
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Side)
        {
            isAttackPlayed = false;
            await PowerCmd.Apply<TransferenceStaticPower>(base.Owner, base.Amount, base.Owner, null);
        }
    }
}

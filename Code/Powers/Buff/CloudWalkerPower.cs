using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Powers.Buff;

public sealed class CloudWalkerPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromPower<IntangiblePower>()
	];

	private class Data
	{
		public int cardsDrawn;
	}

	protected override object InitInternalData()
	{
		return new Data();
	}

	public override bool IsInstanced => true;

	public override int DisplayAmount => base.Amount - GetInternalData<Data>().cardsDrawn % base.Amount;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature == base.Owner)
		{
			Data data = GetInternalData<Data>();
			data.cardsDrawn++;
			if(data.cardsDrawn % base.Amount == 0)
			{
				await CreatureCmd.Heal(base.Owner, 1m);
			}
			InvokeDisplayAmountChanged();
		}
    }

	public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == base.Owner.Side)
		{
			if (GetInternalData<Data>().cardsDrawn == 0)
			{
				await PowerCmd.Apply<IntangiblePower>(base.Owner, 1m, null, null);
			}
            await PowerCmd.Remove(this);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Buff;

public sealed class TurbulencePower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;

    private class Data
	{
		public int cardsDrawn = 0;
	}

    protected override object InitInternalData()
	{
		return new Data();
	}

    public const int Decrement = 10;

    public override int DisplayAmount => Math.Max(0, base.Amount - GetInternalData<Data>().cardsDrawn * Decrement);

	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		GetExtraHoverTip()
	];

	protected override HoverTip GetExtraHoverTip()
	{
		StringBuilder stringBuilder = new();
		LocString locString = ExtraDescription;
        locString.Add("DisplayAmount", DisplayAmount);
        locString.Add("Decrement", Decrement);
		DynamicVars.AddTo(locString);
		stringBuilder.Append(locString.GetFormattedText());
		return new HoverTip(this, stringBuilder.ToString(), true);
	}

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner)
        {
            return 1m;
        }
        if (!props.IsPoweredAttack())
        {
            return 1m;
        }
        return 1m - base.DisplayAmount / 100m;
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == base.Owner.Side)
        {
            GetInternalData<Data>().cardsDrawn = 0;
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;        
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature != base.Owner || fromHandDraw)
        {
            return Task.CompletedTask;
        }
        GetInternalData<Data>().cardsDrawn++;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}

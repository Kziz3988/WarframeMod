using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Cards.Uncommon;

namespace WarframeMod.Code.Powers.Buff;

public sealed class ThuriblePower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;

    private class Data
	{
		public int maxEnergyLost = 0;
        public int totalEnergyGain = 0;
	}

    protected override object InitInternalData()
	{
		return new Data();
	}

    public override int DisplayAmount => GetInternalData<Data>().totalEnergyGain;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		GetExtraHoverTip()
	];

	protected override HoverTip GetExtraHoverTip()
	{
		StringBuilder stringBuilder = new();
		LocString locString = ExtraDescription;
        locString.Add("Amount", base.Amount);
		locString.Add("EnergyCount", DisplayAmount);
        locString.Add("MaxEnergyCount", GetInternalData<Data>().maxEnergyLost);
        locString.Add("energyPrefix", EnergyIconHelper.GetPrefix(this));
		DynamicVars.AddTo(locString);
		stringBuilder.Append(locString.GetFormattedText());
		return new HoverTip(this, stringBuilder.ToString(), true);
	}

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && amount > 0)
        {
            GetInternalData<Data>().maxEnergyLost += 1;
        }
        return Task.CompletedTask;        
    }

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        if (player != base.Owner.Player)
        {
            return amount;
        }
        return amount - GetInternalData<Data>().maxEnergyLost;
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == base.Owner.Side)
        {
            GetInternalData<Data>().totalEnergyGain += base.Amount;
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;        
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature != base.Owner)
        {
            return Task.CompletedTask;
        }
        if (card.GetType() != typeof(Thurible))
        {
            return Task.CompletedTask;
        }
        PlayerCmd.GainEnergy(GetInternalData<Data>().totalEnergyGain, card.Owner);
        PowerCmd.Remove(this);
        return Task.CompletedTask;
    }
}

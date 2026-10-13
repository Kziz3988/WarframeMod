using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Buff;

public sealed class KuvaLichPower : WarframeModPower
{
	private class Data
	{
		public int damageReceived = 0;
        public int damageAmount = 0;
	}

	protected override object InitInternalData()
	{
		return new Data();
	}

    public void InitDamageAmount(int damageAmount)
    {
        GetInternalData<Data>().damageAmount = damageAmount;
        InvokeDisplayAmountChanged();
    }

    public bool ShouldTriggerAfterDamage(int amount)
    {
        Data data = GetInternalData<Data>();
        return data.damageReceived / data.damageAmount != (data.damageReceived + amount) / data.damageAmount;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		GetExtraHoverTip(),
        StunIntent.GetStaticHoverTip(),
        HoverTipFactory.FromPower<RequiemMurmurPower>(),
        HoverTipFactory.FromPower<IntangiblePower>(),
        HoverTipFactory.FromPower<StrengthPower>()
	];

	public override int DisplayAmount => GetInternalData<Data>().damageAmount - (GetInternalData<Data>().damageReceived % GetInternalData<Data>().damageAmount);

    protected override HoverTip GetExtraHoverTip()
	{
		StringBuilder stringBuilder = new();
		LocString locString = ExtraDescription;
		locString.Add("Amount", base.Amount);
        locString.Add("DisplayAmount", DisplayAmount);
        locString.Add("OwnerName", base.Owner.Name);
        locString.Add("OnPlayer", base.Owner.Name);
		DynamicVars.AddTo(locString);
		stringBuilder.Append(locString.GetFormattedText());
		return new HoverTip(this, stringBuilder.ToString(), true);
	}

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner && result.UnblockedDamage > 0)
        {
            if (ShouldTriggerAfterDamage(result.UnblockedDamage))
            {
                Flash();
                await CreatureCmd.Stun(base.Owner);
                await PowerCmd.Apply<RequiemMurmurPower>(choiceContext, base.Owner, 1m, base.Owner, null);
            }
            GetInternalData<Data>().damageReceived += result.UnblockedDamage;
            InvokeDisplayAmountChanged();
        }
    }

}

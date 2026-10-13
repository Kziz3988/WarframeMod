using System.Collections.Generic;
using System.Text;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace WarframeMod.Code.Powers.Buff;

public sealed class RequiemCorrectPower : WarframeModPower
{
    private LocString _cardType = CardType.None.ToLocString();

    public LocString RequiemCardType
    {
        get
        {
            return _cardType;
        }
        set
        {
            AssertMutable();
            _cardType = value;
            InvokeDisplayAmountChanged();
        }
    }

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        GetExtraHoverTip(),
        HoverTipFactory.FromPower<RequiemMurmurPower>()
    ];

    protected override HoverTip GetExtraHoverTip()
	{
		StringBuilder stringBuilder = new();
		LocString locString = ExtraDescription;
		locString.Add("Amount", base.Amount);
        locString.Add("CardType", RequiemCardType);
		DynamicVars.AddTo(locString);
		stringBuilder.Append(locString.GetFormattedText());
		return new HoverTip(this, stringBuilder.ToString(), true);
	}
}

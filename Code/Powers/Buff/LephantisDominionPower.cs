
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Powers.Buff;

public partial class LephantisDominionPower : WarframeModPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private class Data
	{
		public int turnCounter = 3;
	}

	protected override object InitInternalData()
	{
		return new Data();
	}

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override int DisplayAmount => GetInternalData<Data>().turnCounter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        GetExtraHoverTip(),
        HoverTipFactory.FromPower<IntangiblePower>()
    ];

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

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(base.Owner))
		{
			return;
		}
        if (GetInternalData<Data>().turnCounter == 1)
        {
            await PowerCmd.Apply<IntangiblePower>(choiceContext, base.Owner, 1m, null, null);
            GetInternalData<Data>().turnCounter = 3;
        }
        else
        {
            GetInternalData<Data>().turnCounter--;
        }
        InvokeDisplayAmountChanged();
    }
}
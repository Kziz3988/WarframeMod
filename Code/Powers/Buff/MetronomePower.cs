using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Powers.Buff;

public sealed class MetronomePower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		GetExtraHoverTip(),
		HoverTipFactory.FromPower<StrengthPower>()
	];

	protected override HoverTip GetExtraHoverTip()
	{
		StringBuilder stringBuilder = new();
		LocString locString = ExtraDescription;
        locString.Add("Amount", base.Amount);
        locString.Add("TempStrengthPower", TempStrengthPower);
		DynamicVars.AddTo(locString);
		stringBuilder.Append(locString.GetFormattedText());
		return new HoverTip(this, stringBuilder.ToString(), true);
	}

    private decimal TempStrengthPower { get; set; }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner == base.Owner.Player)
		{
			switch (cardPlay.Card.Type)
            {
            case CardType.Attack:
                await PowerCmd.Apply<StrengthPower>(base.Owner, base.Amount, base.Owner, null);
                TempStrengthPower += base.Amount;
                break;
            case CardType.Skill:
                await CardPileCmd.Draw(context, base.Amount, base.Owner.Player);
                break;
            }
		}
	}

	public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == base.Owner.Side)
		{
            await PowerCmd.Apply<StrengthPower>(base.Owner, -TempStrengthPower, null, null);
			await PowerCmd.Remove(this);
		}
	}
}

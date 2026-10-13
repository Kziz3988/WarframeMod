using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using WarframeMod.Code.Cards;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Powers.Buff;

public sealed class ElementalWardPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		ElementData.GetStaticHoverTip(),
        HoverTipFactory.FromPower<ShieldPower>()
	];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player.Creature == base.Owner && (cardPlay.Card.Tags.Contains((CardTag)WarframeModCardTag.Element) || cardPlay.Card.GetElements().Amount > 0))
        {
            Flash();
            await ShieldPower.ApplyShield(base.Owner, base.Amount, 0, 0, base.Owner, null);
        }
    }
}

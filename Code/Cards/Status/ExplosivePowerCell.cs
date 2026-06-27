using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace WarframeMod.Code.Cards.Status;

[Pool(typeof(StatusCardPool))]
public class ExplosivePowerCell() : WarframeModCard(-1, CardType.Status, CardRarity.Status, TargetType.None)
{
    public override bool CanBeGeneratedInCombat => false;
	public override int MaxUpgradeLevel => 0;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card != this || oldPileType != PileType.Hand)
        {
            return Task.CompletedTask;
        }
        if (card.Pile?.Type == PileType.Discard)
        {
            AddKeyword(CardKeyword.Ethereal);
            CardCmd.Preview(this, 0.5f);
        }
        return Task.CompletedTask;
    }
}
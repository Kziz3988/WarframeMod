using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Cards.Uncommon;

public class WrathfulAdvance() : WarframeModCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StrengthPower>(1m),
        new CardsVar(1)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner.Creature, base.DynamicVars.Strength.BaseValue, base.Owner.Creature, this);
        CardModel cardModel;
        int cardCount = 0;
        CardModel[] cards = new CardModel[base.DynamicVars.Cards.IntValue];
        do
		{
			cardModel = await CardPileCmd.Draw(choiceContext, base.Owner);
            if (cardModel != null && cardModel.Type == CardType.Attack)
            {
                cards[cardCount] = cardModel;
                cardCount++;
            }
		}
		while (cardModel != null && cardCount < base.DynamicVars.Cards.IntValue && CardPile.GetCards(base.Owner, PileType.Hand).Count() < 10);
        foreach (CardModel card in cards)
        {
            if (card != null && card.Pile?.Type == PileType.Hand)
            {
                await CardCmd.AutoPlay(choiceContext, card, null);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
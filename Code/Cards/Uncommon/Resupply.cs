using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Cards.Uncommon;

public class Resupply() : WarframeModCard(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ElementData.GetStaticHoverTip()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        List<CardModel> cardPool = CardFactory.GetDistinctForCombat(base.Owner, from c in base.Owner.Character.CardPool.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
			where c.GetType() != typeof(Resupply) && c.Tags.Contains((CardTag)WarframeModCardTag.Element) && (c.Rarity == CardRarity.Common || c.Rarity == CardRarity.Uncommon || c.Rarity == CardRarity.Rare)
			select c, base.DynamicVars.Cards.IntValue, base.Owner.RunState.Rng.CombatCardGeneration)
            .ToList();
        foreach (CardModel card in cardPool)
        {
            card.SetToFreeThisTurn();
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
        }
	}

	protected override void OnUpgrade()
	{
		base.DynamicVars.Cards.UpgradeValueBy(1m);
	}
}
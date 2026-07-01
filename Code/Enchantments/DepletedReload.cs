
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Enchantments;

public sealed class DepletedReload : WarframeModEnchantment
{
    protected override string? CustomIconPath => "depleted_reload.png".EnchantmentUiPath();

    public override bool HasExtraCardText => true;

	public override bool ShowAmount => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(4), new IntVar("Increment", 5)];

    public override bool CanEnchant(CardModel card)
    {
        if (!base.CanEnchant(card))
        {
            return false;
        }
        return card.Type == CardType.Skill;
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        await CardCmd.Discard(choiceContext, await CardSelectCmd.FromHandForDiscard(choiceContext, base.Card.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, base.DynamicVars.Cards.IntValue), null, this));
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, base.Card.Owner.Creature, base.DynamicVars["Increment"].BaseValue, base.Card.Owner.Creature, base.Card);
    }
}
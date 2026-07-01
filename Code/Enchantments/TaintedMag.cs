
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Enchantments;

public sealed class TaintedMag : WarframeModEnchantment
{
    protected override string? CustomIconPath => "tainted_mag.png".EnchantmentUiPath();

    public override bool HasExtraCardText => true;

	public override bool ShowAmount => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new IntVar("Decrement", 1)];

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
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Card.Owner);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, base.Card.Owner.Creature, -base.DynamicVars["Decrement"].BaseValue, base.Card.Owner.Creature, base.Card);
    }
}
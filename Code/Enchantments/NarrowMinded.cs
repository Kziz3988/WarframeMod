
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Enchantments;

public sealed class NarrowMinded : WarframeModEnchantment
{
    protected override string? CustomIconPath => "narrow_minded.png".EnchantmentUiPath();

    public override bool HasExtraCardText => true;

	public override bool ShowAmount => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override bool CanEnchant(CardModel card)
    {
        if (!base.CanEnchant(card))
        {
            return false;
        }
        return card.Type == CardType.Skill && !card.GetKeywordsWithSources(KeywordSources.Local).Contains(CardKeyword.Exhaust);
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Card.Owner);
    }

    protected override void OnEnchant()
	{
		base.Card.AddKeyword(CardKeyword.Exhaust);
	}
}
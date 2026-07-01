
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Enchantments;

public sealed class BlindRage : WarframeModEnchantment
{
    protected override string? CustomIconPath => "blind_rage.png".EnchantmentUiPath();

    public override bool HasExtraCardText => true;

	public override bool ShowAmount => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("Increment", 100),
        new EnergyVar(1)
    ];

    public override bool CanEnchant(CardModel card)
    {
        if (!base.CanEnchant(card))
        {
            return false;
        }
        return card.Type == CardType.Attack;
    }

    public override decimal EnchantDamageMultiplicative(decimal originalDamage, ValueProp props)
    {
		if (!props.IsPoweredAttack())
		{
			return 1m;
		}
		return 1m + base.DynamicVars["Increment"].BaseValue / 100m;
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        await PlayerCmd.LoseEnergy(base.DynamicVars.Energy.BaseValue, base.Card.Owner);
    }
}
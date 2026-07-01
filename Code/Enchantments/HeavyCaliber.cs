
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Enchantments;

public sealed class HeavyCaliber : WarframeModEnchantment
{
    protected override string? CustomIconPath => "heavy_caliber.png".EnchantmentUiPath();

    public override bool HasExtraCardText => true;

	public override bool ShowAmount => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Increment", 50)];

    public override bool CanEnchant(CardModel card)
    {
        if (!base.CanEnchant(card))
        {
            return false;
        }
        return card.Type == CardType.Attack && card.TargetType == TargetType.AnyEnemy;
    }

    public override decimal EnchantDamageMultiplicative(decimal originalDamage, ValueProp props)
    {
		if (!props.IsPoweredAttack())
		{
			return 1m;
		}
		return 1m + base.DynamicVars["Increment"].BaseValue / 100m;
    }
}
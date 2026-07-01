
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Enchantments;

public sealed class Overextended : WarframeModEnchantment
{
    private bool _isPlaying = false;

    public bool IsPlaying
    {
        get
        {
            return _isPlaying;
        }
        set
        {
            AssertMutable();
            _isPlaying = value;
        }
    }

    protected override string? CustomIconPath => "overextended.png".EnchantmentUiPath();

    public override bool HasExtraCardText => true;

	public override bool ShowAmount => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Decrement", 60)];

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
		return 1m - base.DynamicVars["Decrement"].BaseValue / 100m;
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (!IsPlaying)
        {
            IsPlaying = true;
            foreach (Creature enemy in base.Card.CombatState.HittableEnemies)
            {
                if (enemy == cardPlay?.Target)
                {
                    continue;
                }
                await CardCmd.AutoPlay(choiceContext, base.Card, enemy);
            }
            IsPlaying = false;       
        }
    }
}
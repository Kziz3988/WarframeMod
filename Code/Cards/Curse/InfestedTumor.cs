using System.Collections.Generic;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Cards.Status;

[Pool(typeof(CurseCardPool))]
public class InfestedTumor() : WarframeModCard(1, CardType.Curse, CardRarity.Curse, TargetType.None)
{
    public override bool CanBeGeneratedInCombat => false;
	public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (this.Pile?.Type != PileType.Hand || dealer != base.Owner.Creature || !props.IsPoweredAttack())
        {
            return 1m;
        }
        return 0m;
    }
}
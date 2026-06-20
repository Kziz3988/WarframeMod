using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Event;

[Pool(typeof(EventCardPool))]
public class RebuildShield() : WarframeModCard(1, CardType.Skill, CardRarity.Event, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ShieldPower>()
    ];

	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0),
        new CalculationExtraVar(1m),
        new CalculatedVar("TotalShield").WithMultiplier((CardModel card, Creature? _) => GetShieldAmount(card.Owner.Creature))
    ];

    private static int GetShieldAmount(Creature? creature)
    {
        if (creature == null)
        {
            return 0;
        }
        ShieldPower? shield = creature.GetPower<ShieldPower>();
        if (shield == null)
        {
            return 0;
        }
        return Math.Max(0, shield.ShieldCapacity - shield.TotalShield);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		ShieldPower? shield = base.Owner.Creature.GetPower<ShieldPower>();
        if (shield == null)
        {
            return;
        }
        int shieldAmount = (int)((CalculatedVar)base.DynamicVars["TotalShield"]).Calculate(base.Owner.Creature);
        await ShieldPower.ApplyShield(base.Owner.Creature, shieldAmount, 0, 0, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}

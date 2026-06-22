using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Uncommon;

public class Penance() : WarframeModCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ShieldPower>()
	];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0m),
        new DynamicVar("TotalShield", 2m),
        new CalculationExtraVar(1m),
        new CalculatedVar("Cards").WithMultiplier((CardModel card, Creature? _) => Math.Floor(card.Owner.Creature.GetPower<ShieldPower>()?.TotalShield / card.DynamicVars["TotalShield"].BaseValue ?? 0))
	];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ShieldPower? shield = base.Owner.Creature.GetPower<ShieldPower>();
        if (shield == null)
        {
            return;
        }
        decimal count = ((CalculatedVar)base.DynamicVars["Cards"]).Calculate(base.Owner.Creature);
        await CardPileCmd.Draw(choiceContext, count, base.Owner);
        await ShieldPower.ApplyShield(base.Owner.Creature, -shield.TotalShield, 0, 0, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.RemoveKeyword(CardKeyword.Ethereal);
    }
}

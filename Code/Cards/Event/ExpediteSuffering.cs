using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Event;

[Pool(typeof(EventCardPool))]
public class ExpediteSuffering() : WarframeModCard(1, CardType.Skill, CardRarity.Event, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.FromPower<SlashPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Unpowered | ValueProp.Move).WithMultiplier((CardModel _, Creature? target) => GetTotalDamage(target))
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];

    private static decimal GetTotalDamage(Creature? target)
    {
        if (target == null)
        {
            return 0;
        }
		decimal poison = CalculateDotDamage(target.GetPowerAmount<PoisonPower>());
        decimal slash = CalculateDotDamage(target.GetPowerAmount<SlashPower>());
        return poison + slash;
    }

    private static decimal CalculateDotDamage(decimal amount)
    {
        return amount * (amount + 1) / 2;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        PoisonPower? poisonPower = cardPlay.Target.GetPower<PoisonPower>();
        SlashPower? slashPower = cardPlay.Target.GetPower<SlashPower>();
        await CreatureCmd.Damage(choiceContext, cardPlay.Target, base.DynamicVars.CalculatedDamage.BaseValue, ValueProp.Unpowered, null, this);
        await PowerCmd.Remove(poisonPower);
        await PowerCmd.Remove(slashPower);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}

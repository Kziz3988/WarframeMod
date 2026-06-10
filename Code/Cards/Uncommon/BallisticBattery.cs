using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Cards.Uncommon;

public class BallisticBattery() : WarframeModCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(5m, ValueProp.Move),
        new DynamicVar("Cards", 1m),
        new DynamicVar("ExtraDamage", ExtraDamage)
	];

    private decimal _extraDamage = 0m;
    private decimal ExtraDamage
	{
		get
		{
			return _extraDamage;
		}
		set
		{
			AssertMutable();
			_extraDamage = value;
		}
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        base.DynamicVars.Damage.BaseValue -= ExtraDamage;
        ExtraDamage = 0;
		CardPile pile = PileType.Hand.GetPile(base.Owner);
        for (int i = 0; i < base.DynamicVars["Cards"].IntValue; i++)
        {
            CardModel cardModel = base.Owner.RunState.Rng.CombatCardSelection.NextItem(
                pile.Cards.Where((CardModel c) => c.Type == CardType.Attack)
            );
            if (cardModel != null)
            {
                decimal damage = cardModel.GetDamage();
                ExtraDamage += damage;
                await CardCmd.Exhaust(choiceContext, cardModel);
            }
        }
        base.DynamicVars.Damage.BaseValue += ExtraDamage;
    }

    protected override void AfterDowngraded()
	{
		base.AfterDowngraded();
		base.DynamicVars.Damage.BaseValue += ExtraDamage;
	}

    protected override void OnUpgrade()
    {
		base.DynamicVars["Cards"].UpgradeValueBy(1m);
    }
}

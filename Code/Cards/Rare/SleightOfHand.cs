using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Cards.Rare;

public class SleightOfHand() : WarframeModCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7m, ValueProp.Move),
        new CardsVar(1)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        List<CardModel> cardsIn = (from c in PileType.Draw.GetPile(base.Owner).Cards
			orderby c.Rarity, c.Id
			select c).ToList();
		List<CardModel> list = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, base.DynamicVars.Cards.IntValue))).ToList();
		foreach (CardModel item in list)
		{
			await TransformTo(item, this.CreateClone());
		}
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}

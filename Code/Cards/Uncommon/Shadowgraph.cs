using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Cards.Uncommon;

public class Shadowgraph() : WarframeModCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4m, ValueProp.Move),
		new DynamicVar("Choices", 1m)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        List<CardModel> cards = CardFactory.GetDistinctForCombat(base.Owner, from c in base.Owner.Character.CardPool.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
        where (c.GetType() != typeof(Shadowgraph)) && (c.Rarity == CardRarity.Common || c.Rarity == CardRarity.Uncommon || c.Rarity == CardRarity.Rare)
        select c, base.DynamicVars["Choices"].IntValue, base.Owner.RunState.Rng.CombatCardGeneration).ToList();
        CardModel? card = null;
        if (IsUpgraded)
        {
            card = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, base.Owner, canSkip: true);         
        }
        else{
            card = cards.FirstOrDefault();
        }
        if (card != null)
        {
            card.SetToFreeThisTurn();
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        base.DynamicVars["Choices"].UpgradeValueBy(1m);
    }
}

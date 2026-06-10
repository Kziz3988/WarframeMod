using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Cards.Rare;

public class Kumihimo() : WarframeModCard(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ElementData.GetStaticHoverTip()
    ];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DynamicVar("Choices", 2m),
		new DynamicVar("Chances", 3m)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        HashSet<int> costs = new();
        int costSum = 0;
        ElementData elements = new();
        for (int i = 0; i < base.DynamicVars["Chances"].IntValue; i++)
        {
            List<CardModel> cards = CardFactory.GetDistinctForCombat(base.Owner, from c in base.Owner.Character.CardPool.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
			where (c.GetType() != typeof(Kumihimo)) && c.Tags.Contains((CardTag)WarframeModCardTag.Element) && (c.Rarity == CardRarity.Common || c.Rarity == CardRarity.Uncommon || c.Rarity == CardRarity.Rare)
			select c, base.DynamicVars["Choices"].IntValue, base.Owner.RunState.Rng.CombatCardGeneration)
            .ToList();
            CardModel? chosenCard = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, base.Owner, canSkip: true);
            if (chosenCard != null)
            {
                int cost = chosenCard.EnergyCost.GetAmountToSpend();
                costs.Add(cost);
                costSum += cost;
                elements += chosenCard.GetElements();
                await CardPileCmd.AddGeneratedCardToCombat(chosenCard, PileType.Hand, addedByPlayer: true);
            }
            else
            {
                // Not making a choice is a choice.
                costs.Add(-1);
            }
        }
        await DamageCmd.Attack(costSum).FromCard(this)
			.TargetingAllOpponents(base.CombatState)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
        if (costs.Count == 1 && !costs.Contains(-1))
        {
            foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
            {
                await elements.Apply(hittableEnemy, base.Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Choices"].UpgradeValueBy(1m);
    }
}

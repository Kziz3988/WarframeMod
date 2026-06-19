using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Random;
using WarframeMod.Code.Powers.Buff;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Rare;

public class TransmutationProbe() : WarframeModCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ElectricityPower>(),
        StunIntent.GetStaticHoverTip()
    ];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1),
        new PowerVar<TransmutationProbePower>(1m)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> cardPool = base.Owner.Character.CardPool.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint).Where(delegate(CardModel c)
        {
            CardRarity rarity = c.Rarity;
            bool flag = (c.GetType() != typeof(TransmutationProbe)) && (c.Type == CardType.Attack) && (rarity == CardRarity.Common || rarity == CardRarity.Uncommon || rarity == CardRarity.Rare);
            return flag;
        }).ToList();
        if (cardPool.Count > 0)
        {
            Rng combatCardGeneration = base.Owner.RunState.Rng.CombatCardGeneration;
            CardModel[] cards = [CardFactory.GetDistinctForCombat(base.Owner, cardPool, base.DynamicVars.Cards.IntValue, combatCardGeneration).First()];
            await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, base.Owner);
        }
        await PowerCmd.Apply<TransmutationProbePower>(choiceContext, base.Owner.Creature, base.DynamicVars["TransmutationProbePower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
        base.DynamicVars["TransmutationProbePower"].UpgradeValueBy(1m);
    }
}

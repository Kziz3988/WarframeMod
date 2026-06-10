using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace WarframeMod.Code.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Sproudling() : WarframeModCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1),
        new DynamicVar("CardsAmount", 5m)
    ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    private void UpdateCost()
    {
        int exhaustPileCount = PileType.Exhaust.GetPile(base.Owner).Cards.Count;
        base.EnergyCost.SetThisCombat(base.DynamicVars.Energy.IntValue * (exhaustPileCount / base.DynamicVars["CardsAmount"].IntValue));
    }

    public override Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
    {
        UpdateCost();
        return Task.CompletedTask;
    }

    public override Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        UpdateCost();
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["CardsAmount"].UpgradeValueBy(3m);
    }
}

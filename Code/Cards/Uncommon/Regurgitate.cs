using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Cards.Uncommon;

public class Regurgitate() : WarframeModCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>()];
	protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal amount = 0;
        for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
        {
            CardModel? card = PileType.Exhaust.GetPile(base.Owner).Cards
                .Where((CardModel c) => c.Type == CardType.Attack && !c.Keywords.Contains(CardKeyword.Unplayable)).ToList()
                .StableShuffle(base.Owner.RunState.Rng.Shuffle)
                .FirstOrDefault();
            if (card != null)
            {
                amount += card.EnergyCost.GetAmountToSpend();
                await CardCmd.AutoPlay(choiceContext, card, null);
            }
        }
        await PowerCmd.Apply<PoisonPower>(choiceContext, base.CombatState.HittableEnemies, amount, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}

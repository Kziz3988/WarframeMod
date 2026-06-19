using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Powers.Buff;

public sealed class TransmutationProbePower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

    private CardModel? cardSource;
    private bool isCardSourceFinished = false;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        isCardSourceFinished = cardSource == null;
        this.cardSource = cardSource;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (!isCardSourceFinished && cardPlay.Card == cardSource)
        {
            isCardSourceFinished = true;
            return;
        }
        if (cardPlay.Card.Owner.Creature == base.Owner && cardPlay.Card.Type == CardType.Attack)
		{
            await PowerCmd.Decrement(this);
		}
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (!isCardSourceFinished && cardSource == this.cardSource)
        {
            return;
        }
        if (cardSource != null && cardSource.Owner.Creature == base.Owner && cardSource.Type == CardType.Attack)
		{
            await PowerCmd.Apply<ElectricityPower>(choiceContext, target, result.UnblockedDamage, base.Owner, null);
		}
    }
}

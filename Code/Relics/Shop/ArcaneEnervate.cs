using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Relics.Shop;

[Pool(typeof(ExcaliburRelicPool))]
public class ArcaneEnervate : WarframeModRelic
{
    private int _attacksPlayed = 0;

    [SavedProperty]
	public int AttacksPlayed
	{
		get
		{
			return _attacksPlayed;
		}
		set
		{
			AssertMutable();
			_attacksPlayed = value;
			InvokeDisplayAmountChanged();
		}
	}

    public override RelicRarity Rarity => RelicRarity.Shop;

    public override bool ShowCounter => true;

    public override int DisplayAmount => AttacksPlayed;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer == base.Owner.Creature && props.IsPoweredAttack() && cardSource?.Type == CardType.Attack)
        {
            return AttacksPlayed;
        }
        return 0m;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != base.Owner)
        {
            return Task.CompletedTask;
        }
        if (cardPlay.Card.Type == CardType.Attack)
        {
            AttacksPlayed++;
        }
        else
        {
            AttacksPlayed = 0;
        }
        return Task.CompletedTask;
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

namespace WarframeMod.Code.Powers.Buff;

public sealed class DispensaryPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

	private class Data
	{
		public int turnCounter = -1;
	}

	protected override object InitInternalData()
	{
		return new Data();
	}

	public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

	public override int DisplayAmount => GetInternalData<Data>().turnCounter % 3 + 1;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != base.Owner.Player)
		{
			return;
		}
		if (base.Owner.IsAlive)
		{
            GetInternalData<Data>().turnCounter++;
            InvokeDisplayAmountChanged();
            switch(GetInternalData<Data>().turnCounter % 3)
            {
            case 0:
                await PlayerCmd.GainEnergy(1m, base.Owner.Player);
                break;
            case 1:
                await CreatureCmd.Heal(base.Owner, 1m);
                break;
            case 2:
            	List<CardModel> cardPool = base.Owner.Player.Character.CardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint).Where(delegate(CardModel c)
                {
                    CardRarity rarity = c.Rarity;
                    bool flag = rarity == CardRarity.Common || rarity == CardRarity.Uncommon || rarity == CardRarity.Rare;
                    return flag;
                }).ToList();
                if (cardPool.Count > 0)
                {
                    Rng combatCardGeneration = base.Owner.Player.RunState.Rng.CombatCardGeneration;
                    CardModel[] cards = [CardFactory.GetDistinctForCombat(player, cardPool, 1, combatCardGeneration).First()];
                    await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, base.Owner.Player);
                }
                break;
            }
		}
		else
		{
			await Cmd.CustomScaledWait(0.1f, 0.25f);
		}
    }
}

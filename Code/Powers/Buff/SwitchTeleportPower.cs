using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace WarframeMod.Code.Powers.Buff;

public sealed class SwitchTeleportPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player)
		{
			return;
		}
        IEnumerable<CardModel> pile = PileType.Draw.GetPile(base.Owner.Player).Cards.Where((CardModel c) => c.Type == CardType.Skill);
		IEnumerable<CardModel> cards = pile.ToList().UnstableShuffle(base.Owner.Player.RunState.Rng.CombatCardSelection).Take(base.Amount);
		foreach (CardModel card in cards)
		{
			await CardPileCmd.Add(card, PileType.Hand);
		}
        IEnumerable<CardModel> selected = await CardSelectCmd.FromHand(choiceContext, base.Owner.Player, new CardSelectorPrefs(base.SelectionScreenPrompt, base.Amount), null, this);
		foreach (CardModel card in selected)
		{
			await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Random);
		}
    }
}

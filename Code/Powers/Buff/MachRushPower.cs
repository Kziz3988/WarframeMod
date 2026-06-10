using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace WarframeMod.Code.Powers.Buff;

public sealed class MachRushPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == base.Owner.Side)
		{
            ArgumentNullException.ThrowIfNull(base.Owner.Player, "base.Owner.Player");
		    List<CardModel> hand = PileType.Hand.GetPile(base.Owner.Player).Cards.ToList();
            foreach (CardModel card in hand)
            {
                await CardCmd.Exhaust(choiceContext, card);
            }
            await PowerCmd.Remove(this);
		}
    }
}

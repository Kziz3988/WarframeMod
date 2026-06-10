using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace WarframeMod.Code.Powers.Buff;

public sealed class CelestialTwinPower : WarframeModPower
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => true;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(base.Owner.Player, "base.Owner.Player");
        if (cardPlay.Card.Owner.Creature == base.Owner && cardPlay.Card.EnergyCost.GetAmountToSpend() >= base.Amount)
        {
            if (cardPlay.Card.Type == CardType.Attack)
            {
                CardModel? card = PileType.Draw.GetPile(base.Owner.Player).Cards
                    .Where((CardModel c) => c.Type == CardType.Skill && !c.Keywords.Contains(CardKeyword.Unplayable)).ToList()
                    .StableShuffle(base.Owner.Player.RunState.Rng.Shuffle)
                    .FirstOrDefault();
                if (card != null)
                {
                    await CardCmd.AutoPlay(context, card, null);
                    await CardCmd.Exhaust(context, card, false, true);
                }
            }
            else if (cardPlay.Card.Type == CardType.Skill)
            {
                CardModel? card = PileType.Draw.GetPile(base.Owner.Player).Cards
                    .Where((CardModel c) => c.Type == CardType.Attack && !c.Keywords.Contains(CardKeyword.Unplayable)).ToList()
                    .StableShuffle(base.Owner.Player.RunState.Rng.Shuffle)
                    .FirstOrDefault();
                if (card != null)
                {
                    await CardCmd.AutoPlay(context, card, null);
                    await CardCmd.Exhaust(context, card, false, true);
                }
            }
        }
    }
}

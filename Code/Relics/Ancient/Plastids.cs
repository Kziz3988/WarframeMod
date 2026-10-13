using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Cards;
using WarframeMod.Code.Cards.Status;
using WarframeMod.Code.Events;

namespace WarframeMod.Code.Relics.Ancient;

public sealed class Plastids : WarframeModRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<InfestedTumor>()];

    public override async Task AfterObtained()
	{
        CardModel original = PileType.Deck.GetPile(base.Owner).Cards
            .Where(c => c.IsTransformable).ToList()
            .StableShuffle(base.Owner.PlayerRng.Transformations)
			.First();
        CardModel card = Helminth.CommonCards.Concat(Helminth.UncommonCards).Concat(Helminth.RareCards).ToList()
            .StableShuffle(base.Owner.PlayerRng.Rewards)
            .First();
        CardModel replacement = base.Owner.RunState.CreateCard(card, base.Owner);
        await WarframeModCard.TransformTo(original, replacement);
        List<CardModel> list = [base.Owner.RunState.CreateCard<InfestedTumor>(base.Owner)];
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(list, PileType.Deck));
	}
}

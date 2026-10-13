using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using WarframeMod.Code.Cards.Event;

namespace WarframeMod.Code.Relics.Ancient;

public sealed class RivenSliver : WarframeModRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Riven>()];

    public override async Task AfterObtained()
	{
        await PlayerCmd.LoseGold(base.Owner.Gold, base.Owner);
		Riven riven = base.Owner.RunState.CreateCard<Riven>(base.Owner);
        riven.Cycle(base.Owner.PlayerRng.Rewards);
		CardCmd.PreviewCardPileAdd([(await CardPileCmd.Add(riven, PileType.Deck))], 2f);
	}
}

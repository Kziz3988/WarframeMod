using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Cards.Basic;

namespace WarframeMod.Code.Relics.Ancient;

public sealed class NewNeowsTalisman : WarframeModRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;
	public override bool HasUponPickupEffect => true;

	public override Task AfterObtained()
	{
		IEnumerable<CardModel> source = PileType.Deck.GetPile(base.Owner).Cards;
		IEnumerable<CardModel?> cards = [
			source.LastOrDefault(c => c.GetType() == typeof(SlashDash)),
			source.LastOrDefault(c => c.GetType() == typeof(RadialBlind))
        ];
		foreach (CardModel? card in cards)
		{
			if (card != null)
			{
				CardCmd.Upgrade(card);
			}
		}
		return Task.CompletedTask;
	}
}

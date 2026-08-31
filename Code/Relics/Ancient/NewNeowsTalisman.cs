using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using WarframeMod.Code.Cards.Basic;

namespace WarframeMod.Code.Relics.Ancient;

public sealed class NewNeowsTalisman : WarframeModRelic
{
	private ModelId? _strike;
	private ModelId? _defend;

	[SavedProperty]
	public ModelId Strike
	{
		get
		{
			return _strike ?? ModelDb.Card<StrikeWarframe>().Id;
		}
		set
		{
			AssertMutable();
			_strike = value;
			((StringVar)base.DynamicVars["Strike"]).StringValue = ModelDb.GetById<CardModel>(value).Title;
		}
	}

	[SavedProperty]
	public ModelId Defend
	{
		get
		{
			return _defend ?? ModelDb.Card<DefendWarframe>().Id;
		}
		set
		{
			AssertMutable();
			_defend = value;
			((StringVar)base.DynamicVars["Defend"]).StringValue = ModelDb.GetById<CardModel>(value).Title;
		}
	}

	public override RelicRarity Rarity => RelicRarity.Ancient;
	public override bool HasUponPickupEffect => true;

	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new StringVar("Strike"),
		new StringVar("Defend")
	];

	public override Task AfterObtained()
	{
		IEnumerable<CardModel> source = PileType.Deck.GetPile(base.Owner).Cards;
		IEnumerable<CardModel?> cards = [
			source.LastOrDefault(c => c.GetType() == ModelDb.GetById<CardModel>(Strike).GetType()),
			source.LastOrDefault(c => c.GetType() == ModelDb.GetById<CardModel>(Defend).GetType())
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

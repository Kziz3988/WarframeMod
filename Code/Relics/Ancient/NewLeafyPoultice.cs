using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using WarframeMod.Code.Cards.Basic;

namespace WarframeMod.Code.Relics.Ancient;

public sealed class NewLeafyPoultice : WarframeModRelic
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
		new MaxHpVar(12m),
		new StringVar("Strike"),
		new StringVar("Defend")
	];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Transform)];

	public override async Task AfterObtained()
	{
		await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars.MaxHp.BaseValue, isFromCard: false);
		IEnumerable<CardModel> source = PileType.Deck.GetPile(base.Owner).Cards;
		CardModel? strike = source.FirstOrDefault(c => c.GetType() == ModelDb.GetById<CardModel>(Strike).GetType());
		CardModel? defend = source.FirstOrDefault(c => c.GetType() == ModelDb.GetById<CardModel>(Defend).GetType());
		List<CardTransformation> target = [];
		if (strike != null)
		{
			target.Add(new CardTransformation( strike));
		}
		if (defend != null)
		{
			target.Add(new CardTransformation(defend));
		}
		await CardCmd.Transform(target, base.Owner.PlayerRng.Transformations);
	}
}

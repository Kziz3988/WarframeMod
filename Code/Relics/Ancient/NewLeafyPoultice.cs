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
using WarframeMod.Code.Cards.Basic;

namespace WarframeMod.Code.Relics.Ancient;

public sealed class NewLeafyPoultice : WarframeModRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;
	protected override IEnumerable<DynamicVar> CanonicalVars => [new MaxHpVar(12m)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Transform)];

	public override async Task AfterObtained()
	{
		await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars.MaxHp.BaseValue, isFromCard: false);
		IEnumerable<CardModel> source = PileType.Deck.GetPile(base.Owner).Cards;
		CardModel? slashDash = source.FirstOrDefault(c => c.GetType() == typeof(SlashDash));
		CardModel? radialBlind = source.FirstOrDefault(c => c.GetType() == typeof(RadialBlind));
		List<CardTransformation> target = [];
		if (slashDash != null)
		{
			target.Add(new CardTransformation(slashDash));
		}
		if (radialBlind != null)
		{
			target.Add(new CardTransformation(radialBlind));
		}
		await CardCmd.Transform(target, base.Owner.PlayerRng.Transformations);
	}
}

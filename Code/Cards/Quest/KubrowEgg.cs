using System.Collections.Generic;
using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models.CardPools;
using WarframeMod.Code.RestSite;

namespace WarframeMod.Code.Cards.Quest;

[Pool(typeof(QuestCardPool))]
public class KubrowEgg() : WarframeModCard(-1, CardType.Quest, CardRarity.Quest, TargetType.None)
{
    public override int MaxUpgradeLevel => 0;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
	{
		if (player != base.Owner || options.Any(o => o.GetType() == typeof(IncubateRestSiteOption)))
		{
			return false;
		}
		options.Add(new IncubateRestSiteOption(player));
		return true;
	}
}

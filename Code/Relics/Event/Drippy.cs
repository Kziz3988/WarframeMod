using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace WarframeMod.Code.Relics.Event;

[Pool(typeof(EventRelicPool))]
public sealed class Drippy : WarframeModRelic
{
	public override RelicRarity Rarity => RelicRarity.Event;
}

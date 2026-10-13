using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace WarframeMod.Code.Relics.Ancient;

public sealed class VoidTraces : WarframeModRelic
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [VoidRelic.GetStaticHovertip()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Relics", 2m)];

    public override async Task AfterObtained()
	{
        for (int i = 0; i < base.DynamicVars["Relics"].IntValue; i++)
        {
            await RelicCmd.Obtain(base.Owner.PlayerRng.Rewards.NextItem(VoidRelic.Relics).ToMutable(), base.Owner);
        }
	}
}

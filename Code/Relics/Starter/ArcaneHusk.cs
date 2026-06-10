using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Relics.Starter;

public class ArcaneHusk : WarframeModRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ShieldPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("TotalShield", 10m),
        new DynamicVar("ShieldCapacity", 10m),
        new DynamicVar("ShieldRecharge", 1m),
    ];

	public override async Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is CombatRoom)
		{
            await ShieldPower.ApplyShield(
                base.Owner.Creature,
                base.DynamicVars["TotalShield"].IntValue,
                base.DynamicVars["ShieldCapacity"].IntValue,
                base.DynamicVars["ShieldRecharge"].IntValue,
                base.Owner.Creature,
                null
            );
		}
	}
}
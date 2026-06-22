using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace WarframeMod.Code.Relics.Event;

public sealed class Lith : VoidRelic
{
    protected override void AddExtraReward(CombatRoom combatRoom)
    {
        combatRoom.AddExtraReward(base.Owner, new PotionReward(base.Owner));
    }
}

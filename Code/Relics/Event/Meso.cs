using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace WarframeMod.Code.Relics.Event;

public sealed class Meso : VoidRelic
{
    protected override void AddExtraReward(CombatRoom combatRoom)
    {
        CardCreationOptions options = CardCreationOptions.ForNonCombatWithDefaultOdds([base.Owner.Character.CardPool]).WithFlags(CardCreationFlags.NoRarityModification | CardCreationFlags.NoCardPoolModifications);
        combatRoom.AddExtraReward(base.Owner, new CardReward(options, 3, base.Owner));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace WarframeMod.Code.Relics;

public abstract class VoidRelic : WarframeModRelic
{
    public const int TotalEnemies = 10;

	public override RelicRarity Rarity => RelicRarity.Event;
	public override bool ShowCounter => DisplayAmount > 0;
    public override int DisplayAmount => Math.Max(0, base.DynamicVars["TotalEnemies"].IntValue - Enemies);

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("TotalEnemies", TotalEnemies),
        new DynamicVar("RemainingEnemies", TotalEnemies),
        new BoolVar("IsTriggered", Convert.ToBoolean(IsTriggered))
    ];

    //Bug: bool type is always deserialized after int type!

    private int _isTriggered = 0;

    [SavedProperty(SerializationCondition.AlwaysSave, -1)]
    public int IsTriggered
    {
		get
		{
			return _isTriggered;
		}
		set
		{
			AssertMutable();
			_isTriggered = value;
            ((BoolVar)base.DynamicVars["IsTriggered"]).BoolVal = Convert.ToBoolean(_isTriggered);
		}
    }

    private int _enemies = 0;

    [SavedProperty(SerializationCondition.AlwaysSave, 1)]
	public int Enemies
	{
		get
		{
			return _enemies;
		}
		set
		{
			AssertMutable();
			_enemies = value;
			InvokeDisplayAmountChanged();
            base.DynamicVars["RemainingEnemies"].BaseValue = DisplayAmount;
            if (Convert.ToBoolean(IsTriggered) && base.Status == RelicStatus.Normal)
            {
                base.Status = RelicStatus.Disabled;
            }
		}
	}

    public static HoverTip GetStaticHovertip()
    {
		return new HoverTip(new LocString("static_hover_tips", "WARFRAMEMOD-VOID_RELIC.title"), new LocString("static_hover_tips", "WARFRAMEMOD-VOID_RELIC.description"));
    }

    protected abstract void AddExtraReward(CombatRoom combatRoom);

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        AbstractRoom? currentRoom = creature.CombatState?.RunState.CurrentRoom ?? null;
		if (currentRoom is not CombatRoom combatRoom)
        {
            return Task.CompletedTask;
        }
        bool shouldTriggerFatal = creature.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
		if (shouldTriggerFatal && creature.Side != base.Owner.Creature.Side)
        {
            Enemies++;
            if (DisplayAmount <= 0 && base.Status == RelicStatus.Normal)
            {
                IsTriggered = 1;
                base.Status = RelicStatus.Active;
                AddExtraReward(combatRoom);
            }
        }
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (base.Status == RelicStatus.Active)
        {
            base.Status = RelicStatus.Disabled;
        }
        return Task.CompletedTask;
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Relics.Event;

public sealed class GrustragBolt : WarframeModRelic
{
    public const int TotalCombats = 1;

	public override RelicRarity Rarity => RelicRarity.Event;
	public override bool ShowCounter => DisplayAmount > 0;
    public override int DisplayAmount => Math.Max(0, base.DynamicVars["TotalCombats"].IntValue - Combats);

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("TotalCombats", TotalCombats),
        new DynamicVar("Decrement", 50m),
        new DynamicVar("RemainingCombats", TotalCombats),
    ];

    private int _combats = 0;

    [SavedProperty]
	public int Combats
	{
		get
		{
			return _combats;
		}
		set
		{
			AssertMutable();
			_combats = value;
			InvokeDisplayAmountChanged();
            base.DynamicVars["RemainingCombats"].BaseValue = DisplayAmount;
            if (DisplayAmount <= 0)
            {
                base.Status = RelicStatus.Disabled;
            }
            else
            {
                base.Status = RelicStatus.Normal;
            }
		}
	}

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (base.Status == RelicStatus.Normal && dealer == base.Owner.Creature && props.IsPoweredAttack() && cardSource != null)
        {
            return 1m - (base.DynamicVars["Decrement"].BaseValue / 100m);
        }
        return 1m;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        Combats++;
        return Task.CompletedTask;
    }
}

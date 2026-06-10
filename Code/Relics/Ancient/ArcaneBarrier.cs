using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Relics.Ancient;

public sealed class ArcaneBarrier : WarframeModRelic
{

	private int _cooldown;
	public override RelicRarity Rarity => RelicRarity.Ancient;
	protected override IEnumerable<IHoverTip> ExtraHoverTips =>[HoverTipFactory.FromPower<ShieldPower>()];
	public override bool ShowCounter => DisplayAmount > 0;
	public override int DisplayAmount
	{
		get
		{
			if (!CombatManager.Instance.IsInProgress)
			{
				return -1;
			}
			if (base.IsCanonical)
			{
				return -1;
			}
			if (_cooldown <= 0)
			{
				return -1;
			}
			return _cooldown;
		}
	}

	private int Cooldown
	{
		get
		{
			return _cooldown;
		}
		set
		{
			AssertMutable();
			_cooldown = value;
			InvokeDisplayAmountChanged();
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Turns", 6m),
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
            ShieldPower? shield = base.Owner.Creature.GetPower<ShieldPower>();
            shield.OnShieldDamaged += AfterShieldDamaged;
		}
	}

    private async void AfterShieldDamaged(ShieldPower shield, int amount, Creature? dealer, CardModel? cardSource)
    {
        if (Cooldown > 0 || amount <= 0 || shield.TotalShield >= shield.ShieldCapacity)
        {
            return;
        }
        await ShieldPower.ApplyShield(base.Owner.Creature, shield.ShieldCapacity - shield.TotalShield, 0, 0, base.Owner.Creature, null);
        Cooldown = base.DynamicVars["Turns"].IntValue;
        base.Status = RelicStatus.Normal;
    }

	public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
	{
		if (side == base.Owner.Creature.Side)
		{
			Cooldown--;
			if (Cooldown <= 0)
			{
				base.Status = RelicStatus.Active;
				InvokeDisplayAmountChanged();
			}
		}
        return Task.CompletedTask;
	}

	public override Task AfterCombatEnd(CombatRoom _)
	{
		base.Status = RelicStatus.Normal;
		Cooldown = 0;
		InvokeDisplayAmountChanged();
        ShieldPower? shield = base.Owner.Creature.GetPower<ShieldPower>();
        shield.OnShieldDamaged -= AfterShieldDamaged;
		return Task.CompletedTask;
	}

    public override Task AfterRemoved()
    {
        ShieldPower? shield = base.Owner.Creature.GetPower<ShieldPower>();
        shield.OnShieldDamaged -= AfterShieldDamaged;
        return Task.CompletedTask;
    }
}

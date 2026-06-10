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

namespace WarframeMod.Code.Relics.Uncommon;

public sealed class ArcaneAegis : WarframeModRelic
{

	private int _cooldown;
	public override RelicRarity Rarity => RelicRarity.Uncommon;
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

    private ShieldPower? shield = null;

	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("ShieldRecharge", 1m),
        new DynamicVar("Turns", 3m),
    ];

	private void Initialize(ShieldPower? shieldPower)
    {
        if (shieldPower == null)
        {
            return;
        }
        shield = shieldPower;
        shield.OnShieldDamaged += AfterShieldDamaged;
		base.Status = RelicStatus.Normal;
        InvokeDisplayAmountChanged();  
	}

	private void TakeEffect()
	{
		if (shield != null && base.Status == RelicStatus.Normal)
        {
            shield.ModifyShield(0, 0, 1);
        }
		Cooldown = base.DynamicVars["Turns"].IntValue;
        base.Status = RelicStatus.Active;
		InvokeDisplayAmountChanged();
	}

	private void LoseEffect()
	{
		if (shield != null && base.Status == RelicStatus.Active)
		{
			shield.ModifyShield(0, 0, -1);
		}
		base.Status = RelicStatus.Normal;
		InvokeDisplayAmountChanged();
	}

	public override Task AfterObtained()
    {
        ShieldPower? shieldPower = base.Owner.Creature.GetPower<ShieldPower>();
        Initialize(shieldPower);
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (shield == null && power.Owner == base.Owner.Creature && power.GetType() == typeof(ShieldPower))
        {
            Initialize(power as ShieldPower);
        }
        return Task.CompletedTask;
    }

    private void AfterShieldDamaged(ShieldPower shield, int amount, Creature? dealer, CardModel? cardSource)
    {
        if (shield.Owner != base.Owner.Creature || amount <= 0 || shield.TotalShield >= shield.ShieldCapacity)
        {
            return;
        }
        TakeEffect();
    }

	public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
	{
		if (side == base.Owner.Creature.Side)
		{
			Cooldown--;
			if (Cooldown <= 0)
			{
                LoseEffect();
			}
		}
        return Task.CompletedTask;
	}

	public override Task AfterCombatEnd(CombatRoom _)
	{
		Cooldown = -1;
		LoseEffect();
        InvokeDisplayAmountChanged();
        if (shield != null)
        {
            shield.OnShieldDamaged -= AfterShieldDamaged;
        }
        shield = null;
		return Task.CompletedTask;
	}

    public override Task AfterRemoved()
    {
		LoseEffect();
        if (shield != null)
        {
            shield.OnShieldDamaged -= AfterShieldDamaged;
        }
        return Task.CompletedTask;
    }
}

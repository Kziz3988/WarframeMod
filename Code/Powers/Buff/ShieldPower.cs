using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Buff;

public sealed class ShieldData
{
    private int _shield;
    private int _maxShield;
    private int _recharge;

    public int TotalShield {
        get {
            return _shield;
        }
        set {
            _shield = Math.Max(0, Math.Min(value, int.MaxValue));
        }
    }

    public int ShieldCapacity {
        get {
            return _maxShield;
        }
        set {
            _maxShield = Math.Max(0, Math.Min(value, int.MaxValue));
        }
    }

    public int ShieldRecharge {
        get {
            return _recharge;
        }
        set {
            _recharge = Math.Max(0, Math.Min(value, int.MaxValue));
        }
    
    }
    public int NormalShield => Math.Min(TotalShield, ShieldCapacity);
    public int OverShield => Math.Max(0, TotalShield - ShieldCapacity);

    public ShieldData(int total = 0, int capacity = 0, int recharge = 0)
    {
        TotalShield = total;
        ShieldCapacity = capacity;
        ShieldRecharge = recharge;
    }
}

public sealed class ShieldPower : WarframeModPower
{
	private class Data(int total = 0, int capacity = 0, int recharge = 0)
    {
        public ShieldData shield = new(total, capacity, recharge);
    }

    protected override object InitInternalData()
	{
		return new Data();
	}

    public event Action<ShieldPower, ShieldData, ShieldData>? OnShieldChanged;
    public event Action<ShieldPower, int, Creature?, CardModel?>? OnShieldDamaged;

    public static async Task ApplyShield(Creature target, int total, int capacity, int recharge, Creature? applier, CardModel? cardSource, bool allowNull = true)
    {
        ShieldPower? shield = target.GetPower<ShieldPower>();
        if (shield != null)
        {
            shield.ModifyShield(total, capacity, recharge);
        }
        else if (allowNull)
        {
            await PowerCmd.Apply<ShieldPower>(target, 1m, applier, cardSource);
            shield = target.GetPower<ShieldPower>();
            shield.SetShield(new ShieldData(total, capacity, recharge));
        }
    }

    public ShieldData GetShield()
    {
        return GetInternalData<Data>().shield;
    }

    public void SetShield(ShieldData shield)
    {
        ShieldData oldShield = GetShield();
        GetInternalData<Data>().shield = shield;
        InvokeDisplayAmountChanged();
		Owner.InvokePowerModified(this, 0, false);
        OnShieldChanged?.Invoke(this, oldShield, shield);
    }

    public void ModifyShield(int total, int capacity, int recharge)
    {
        ShieldData shield = GetShield();
        ShieldData oldShield = new(shield.TotalShield, shield.ShieldCapacity, shield.ShieldRecharge);
        shield.TotalShield += total;
        shield.ShieldCapacity += capacity;
        shield.ShieldRecharge += recharge;
        InvokeDisplayAmountChanged();
		Owner.InvokePowerModified(this, 0, false);
        OnShieldChanged?.Invoke(this, oldShield, shield);
    }

    public void RechargeShield()
    {
        ShieldData shield = GetShield();
        if(shield.TotalShield >= shield.ShieldCapacity)
        {
            return;
        }
        ModifyShield(Math.Min(shield.ShieldRecharge, shield.ShieldCapacity - shield.TotalShield), 0, 0);
    }

    public int TotalShield => GetShield().TotalShield;
    public int ShieldCapacity => GetShield().ShieldCapacity;
    public int ShieldRecharge => GetShield().ShieldRecharge;
    public int NormalShield => GetShield().NormalShield;
    public int OverShield => GetShield().OverShield;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => GetShield().TotalShield;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		GetExtraHoverTip()
	];

	protected override HoverTip GetExtraHoverTip()
	{
		StringBuilder stringBuilder = new();
		LocString locString = ExtraDescription;
		locString.Add("TotalShield", TotalShield);
        locString.Add("ShieldCapacity", ShieldCapacity);
        locString.Add("ShieldRecharge", ShieldRecharge);
        locString.Add("NormalShield", NormalShield);
        locString.Add("OverShield", OverShield);
		DynamicVars.AddTo(locString);
		stringBuilder.Append(locString.GetFormattedText());
		return new HoverTip(this, stringBuilder.ToString(), true);
	}


    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target!= base.Owner || TotalShield <= 0 || props.HasFlag(ValueProp.Unblockable))
        {
            return amount;
        }
        if (amount > TotalShield)
        {
            int totalShield = TotalShield;
            ModifyShield(-TotalShield, 0, 0);
            OnShieldDamaged?.Invoke(this, totalShield, dealer, cardSource);
            return amount - totalShield;
        }
        else
        {
            ModifyShield(-(int)amount, 0, 0);
            OnShieldDamaged?.Invoke(this, (int)amount, dealer, cardSource);
            return 0;
        }
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Side && base.Owner.IsAlive)
        {
            RechargeShield();
        }
        return Task.CompletedTask;
    }
}

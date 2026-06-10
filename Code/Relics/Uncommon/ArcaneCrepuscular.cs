using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Relics.Uncommon;

public class ArcaneCrepuscular : WarframeModRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<InvisiblePower>(),
        HoverTipFactory.FromPower<ShieldPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("ShieldRecharge", 1m)
    ];

    private ShieldPower? shield = null;

    private void Initialize(ShieldPower? shieldPower)
    {
        if (shieldPower == null)
        {
            return;
        }
        shield = shieldPower;
    }

    private void TakeEffect()
	{
		if (shield != null && base.Status == RelicStatus.Normal)
        {
            shield.ModifyShield(0, 0, 1);
        }
        base.Status = RelicStatus.Active;
	}

	private void LoseEffect()
	{
		if (shield != null && base.Status == RelicStatus.Active)
		{
			shield.ModifyShield(0, 0, -1);
		}
		base.Status = RelicStatus.Normal;
	}

    public override Task AfterObtained()
    {
        ShieldPower? shieldPower = base.Owner.Creature.GetPower<ShieldPower>();
        Initialize(shieldPower);
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power.Owner == base.Owner.Creature)
        {
            if (power.GetType() == typeof(ShieldPower) && shield == null)
            {
                Initialize(power as ShieldPower);
            }
            if (power.GetType() == typeof(InvisiblePower) && shield != null)
            {
                if (power.Amount > 0)
                {
                    TakeEffect();
                }
                else
                {
                    LoseEffect();
                }
            }
        }
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
	{
		LoseEffect();
        shield = null;
		return Task.CompletedTask;
	}

    public override Task AfterRemoved()
    {
        LoseEffect();
        return Task.CompletedTask;
    }
}
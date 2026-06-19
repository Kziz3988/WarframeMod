using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace WarframeMod.Code.Powers.Buff;

public sealed class ShieldSatellitePower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ShieldPower>()
    ];

	private ShieldPower? shield = null;
    private CardModel? cardSource;
    private bool isRemoved = false;

    private async Task InitializeAsync(ShieldPower? shieldPower)
    {
        if (shieldPower == null)
        {
            return;
        }
        shield = shieldPower;
        shield.OnShieldDamaged += AfterShieldDamaged;
        await ShieldPower.ApplyShield(base.Owner, 0, 0, 1, base.Applier, cardSource);
        isRemoved = false;
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        this.cardSource = cardSource;
        ShieldPower? shieldPower = base.Owner.GetPower<ShieldPower>();
        if (shieldPower != null)
        {
            await InitializeAsync(shieldPower);
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (shield == null && power.Owner == base.Owner && power.GetType() == typeof(ShieldPower))
        {
            await InitializeAsync(power as ShieldPower);
        }        
    }

    private async void AfterShieldDamaged(ShieldPower power, int amount, Creature? dealer, CardModel? cardSource)
    {
        if (shield == null || power.Owner != base.Owner || amount <= 0)
        {
            return;
        }
        Flash();
        await ShieldPower.ApplyShield(base.Owner, base.Amount, 0, 0, base.Owner, null);
        await PowerCmd.Decrement(this);
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (!isRemoved && shield != null && oldOwner == base.Owner)
        {
            isRemoved = true;
            await ShieldPower.ApplyShield(base.Owner, 0, 0, -1, null, null);
            shield.OnShieldDamaged -= AfterShieldDamaged;
        }
    }
}

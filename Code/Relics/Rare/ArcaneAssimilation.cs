using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Character;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Relics.Rare;

[Pool(typeof(MagRelicPool))]
public sealed class ArcaneAssimilation : WarframeModRelic
{

	private int _cooldown;
	public override RelicRarity Rarity => RelicRarity.Rare;
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
        new DynamicVar("TotalShield", 1m),
        new DynamicVar("Turns", 1m),
    ];

	private void Initialize(ShieldPower? shieldPower)
    {
        if (shieldPower == null)
        {
            return;
        }
        shield = shieldPower;
        shield.OnShieldChanged += AfterShieldChanged;
		base.Status = RelicStatus.Normal;
        InvokeDisplayAmountChanged();  
	}

    private void TakeEffect()
	{
		Cooldown = base.DynamicVars["Turns"].IntValue;
        base.Status = RelicStatus.Active;
		InvokeDisplayAmountChanged();
	}

	private void LoseEffect()
	{
		base.Status = RelicStatus.Normal;
		InvokeDisplayAmountChanged();
	}

	public override Task AfterObtained()
    {
        ShieldPower? shieldPower = base.Owner.Creature.GetPower<ShieldPower>();
        Initialize(shieldPower);
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (shield == null && power.Owner == base.Owner.Creature && power.GetType() == typeof(ShieldPower))
        {
            Initialize(power as ShieldPower);
        }
        return Task.CompletedTask;        
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
	{
		if (shield != null && base.Status == RelicStatus.Active && dealer != null && (dealer == base.Owner.Creature || dealer.PetOwner == base.Owner) && cardSource != null && cardSource.Type == CardType.Attack && props.IsPoweredAttack() && result.TotalDamage > 0)
		{
            Flash();
			await ShieldPower.ApplyShield(base.Owner.Creature, base.DynamicVars["TotalShield"].IntValue, 0, 0, base.Owner.Creature, null);
		}
	}

    private void AfterShieldChanged(ShieldPower power, ShieldData old, ShieldData current)
    {
        if (power.Owner == base.Owner.Creature && power == shield && current.TotalShield <= 0 && old.TotalShield > 0)
        {
            TakeEffect();
        }
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner.Creature))
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
            shield.OnShieldChanged -= AfterShieldChanged;
        }
        shield = null;
		return Task.CompletedTask;
	}

    public override Task AfterRemoved()
    {
		LoseEffect();
        if (shield != null)
        {
            shield.OnShieldChanged -= AfterShieldChanged;
        }
        return Task.CompletedTask;
    }
}

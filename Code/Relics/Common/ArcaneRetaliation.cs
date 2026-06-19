using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Relics.Common;

public class ArcaneRetaliation : WarframeModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ShieldPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("TotalShield", 5m),
        new DamageVar(3m, ValueProp.Unpowered | ValueProp.SkipHurtAnim)
    ];

    private ShieldPower? shield = null;
    private int DamageCount => shield?.TotalShield / base.DynamicVars["TotalShield"].IntValue ?? 0;

    public override bool ShowCounter => shield != null;
    public override int DisplayAmount => DamageCount;

    private void Initialize(ShieldPower? shieldPower)
    {
        if (shieldPower == null)
        {
            return;
        }
        shield = shieldPower;
        shield.OnShieldChanged += AfterShieldChanged;
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

    private void AfterShieldChanged(ShieldPower power, ShieldData old, ShieldData current)
    {
        if (power.Owner != base.Owner.Creature)
        {
            return;
        }
        if (DamageCount > 0)
        {
            base.Status = RelicStatus.Active;
        }
        else{
            base.Status = RelicStatus.Normal;
        }
        InvokeDisplayAmountChanged();
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (shield != null && side == base.Owner.Creature.Side)
        {
            for (int i = 0; i < DamageCount; i++)
            {
                Creature? creature = base.Owner.RunState.Rng.CombatTargets.NextItem(base.Owner.Creature.CombatState.HittableEnemies);
                if (creature != null)
                {
                    await CreatureCmd.Damage(choiceContext, creature, base.DynamicVars.Damage, base.Owner.Creature);
                }
            }
        }
    }

    public override Task AfterCombatEnd(CombatRoom _)
	{
		base.Status = RelicStatus.Normal;
        if (shield != null)
        {
            shield.OnShieldChanged -= AfterShieldChanged;
        }
        shield = null;
		return Task.CompletedTask;
	}

    public override Task AfterRemoved()
    {
        if (shield != null)
        {
            shield.OnShieldChanged -= AfterShieldChanged;
        }
        return Task.CompletedTask;
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Relics.Rare;

public class ArcaneMelt : WarframeModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<HeatPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<HeatPower>(10m)
    ];

    private int heatCounter = 0;
    private int effectCounter = 0;
    private bool isApplyingEffect = false;

    public override bool ShowCounter => heatCounter >= 0;

    public override int DisplayAmount => heatCounter % base.DynamicVars["HeatPower"].IntValue;

    public override Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is CombatRoom)
		{
            heatCounter = 0;
            effectCounter = 0;
            isApplyingEffect = false;
		}
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
	}

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (isApplyingEffect) 
        {
            return;
        }
        if (power.GetType() == typeof(HeatPower) && amount > 0 && applier == base.Owner.Creature)
        {
            heatCounter += (int)amount;
            int targetEffectCounter = heatCounter / base.DynamicVars["HeatPower"].IntValue;
            decimal deltaEffect = targetEffectCounter - effectCounter;
            if (deltaEffect > 0)
            {
                Flash();
                Creature? creature = base.Owner.RunState.Rng.CombatTargets.NextItem(base.Owner.Creature.CombatState.HittableEnemies);
                if (creature != null)
                {
                    isApplyingEffect = true;
                    await PowerCmd.Apply<HeatPower>(creature, deltaEffect, base.Owner.Creature, null);
                    isApplyingEffect = false;
                }
                effectCounter = targetEffectCounter;
            }
            InvokeDisplayAmountChanged();
        }
    }

    public override Task AfterCombatEnd(CombatRoom _)
	{
        heatCounter = -1;
		InvokeDisplayAmountChanged();
        return Task.CompletedTask;
	}
}
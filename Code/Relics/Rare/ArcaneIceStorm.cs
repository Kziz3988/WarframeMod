using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Rooms;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Relics.Rare;

public class ArcaneIceStorm : WarframeModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ColdPower>(),
        StunIntent.GetStaticHoverTip(),
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ColdPower>(20m),
        new PowerVar<StrengthPower>(1m)
    ];

    private int coldCounter = 0;
    private int StrengthCounter = 0;
    private bool isApplyingEffect = false;

    public override bool ShowCounter => coldCounter >= 0;

    public override int DisplayAmount => coldCounter % base.DynamicVars["ColdPower"].IntValue;

    public override Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is CombatRoom)
		{
            coldCounter = 0;
            StrengthCounter = 0;
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
        if (power.GetType() == typeof(ColdPower) && amount > 0 && applier == base.Owner.Creature)
        {
            coldCounter += (int)amount;
            int targetStrengthCounter = coldCounter / base.DynamicVars["ColdPower"].IntValue;
            decimal deltaStrength = (targetStrengthCounter - StrengthCounter) * base.DynamicVars.Strength.BaseValue;
            if (deltaStrength > 0)
            {
                Flash();
                isApplyingEffect = true;
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, deltaStrength, base.Owner.Creature, null);
                isApplyingEffect = false;
                StrengthCounter = targetStrengthCounter;
            }
            InvokeDisplayAmountChanged();
        }
    }

    public override Task AfterCombatEnd(CombatRoom _)
	{
        coldCounter = -1;
		InvokeDisplayAmountChanged();
        return Task.CompletedTask;
	}
}
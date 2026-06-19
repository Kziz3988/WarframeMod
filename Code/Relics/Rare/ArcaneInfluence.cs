using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Rooms;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Relics.Rare;

public class ArcaneInfluence : WarframeModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ElectricityPower>(),
        StunIntent.GetStaticHoverTip()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ElectricityPower>(5m)
    ];

    private int electricityCounter = 0;
    private int effectCounter = 0;
    private bool isApplyingEffect = false;

    public override bool ShowCounter => electricityCounter >= 0;

    public override int DisplayAmount => electricityCounter % base.DynamicVars["ElectricityPower"].IntValue;

    public override Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is CombatRoom)
		{
            electricityCounter = 0;
            effectCounter = 0;
            isApplyingEffect = false;
		}
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
	}

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (isApplyingEffect)
        {
            return;
        }
        if (power.GetType() == typeof(ElectricityPower) && amount > 0 && applier == base.Owner.Creature)
        {
            electricityCounter += (int)amount;
            int targetEffectCounter = electricityCounter / base.DynamicVars["ElectricityPower"].IntValue;
            decimal deltaEffect = targetEffectCounter - effectCounter;
            if (deltaEffect > 0)
            {
                Flash();
                isApplyingEffect = true;
                await PowerCmd.Apply<ElectricityPower>(choiceContext, base.Owner.Creature.CombatState.HittableEnemies, deltaEffect, base.Owner.Creature, null);
                isApplyingEffect = false;
                effectCounter = targetEffectCounter;
            }
            InvokeDisplayAmountChanged();
        }        
    }

    public override Task AfterCombatEnd(CombatRoom _)
	{
        electricityCounter = -1;
		InvokeDisplayAmountChanged();
        return Task.CompletedTask;
	}
}
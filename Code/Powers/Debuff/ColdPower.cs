using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Debuff;

public partial class ColdPower : WarframeModPower
{
	public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		StunIntent.GetStaticHoverTip()
	];

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
		if (!props.IsPoweredAttack())
		{
			return 1m;
		}
        if (target == base.Owner)
        {
            return 1.15m;
        }
        if (dealer == base.Owner)
        {
            return 0.85m;
        }
		return 1m;
    }

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        for (int i = 0; i < base.Amount / 10; i++)
        {
            Flash();
            await Stun(base.Owner);
        }
    }

    public override async Task BeforePowerAmountChanged(PowerModel power, decimal amount, Creature target, Creature? applier, CardModel? cardSource)
    {
        if (power == this)
        {
            for (int i = 0; i < (base.Amount + (int)amount) / 10; i++)
            {
                Flash();
                await Stun(base.Owner);
            }
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this)
        {
            this.SetAmount(base.Amount % 10);
            if (base.Amount == 0)
            {
                await PowerCmd.Remove(this);
            }
        }
    }

	public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
		if (side != base.Owner.Side)
		{
			return;
		}
		if (base.Owner.IsAlive)
		{
			await PowerCmd.Decrement(this);
		}
		else
		{
			await Cmd.CustomScaledWait(0.1f, 0.25f);
		}        
    }
}

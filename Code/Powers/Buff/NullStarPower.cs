using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Buff;

public sealed class NullStarPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (!props.IsPoweredAttack())
		{
			return 1m;
		}
		if (dealer == null)
		{
			return 1m;
		}
		if (target != base.Owner)
		{
			return 1m;
		}
		return Math.Max(0, 1m - base.Amount / 100m);
	}

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner && dealer != null && (props.IsPoweredAttack() || cardSource is Omnislice))
        {
            Flash();
            await CreatureCmd.Damage(choiceContext, dealer, 3m, ValueProp.Unpowered | ValueProp.SkipHurtAnim, base.Owner, null);
            await PowerCmd.ModifyAmount(choiceContext, this, -10, null, null);
        }
    }
}

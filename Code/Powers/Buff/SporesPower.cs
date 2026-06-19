using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Buff;

public sealed class SporesPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        GetExtraHoverTip(),
		HoverTipFactory.FromPower<FrailPower>()
	];

    public const decimal Increment = 15m;

    protected override HoverTip GetExtraHoverTip()
	{
		StringBuilder stringBuilder = new();
		LocString locString = ExtraDescription;
        locString.Add("Amount", base.Amount);
        locString.Add("Increment", Increment);
		DynamicVars.AddTo(locString);
		stringBuilder.Append(locString.GetFormattedText());
		return new HoverTip(this, stringBuilder.ToString(), true);
	}

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (!props.IsPoweredAttack())
		{
			return 1m;
		}
		if (cardSource == null || cardSource.Owner.Creature != base.Owner)
		{
			return 1m;
		}
		if (target == null || target.GetPower<FrailPower>() == null)
		{
			return 1m;
		}
		return 1 + Increment / 100m;
	}

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != base.Owner.Side)
        {
            return;
        }
        IReadOnlyList<Creature> hittableEnemies = base.CombatState.HittableEnemies;
        if (hittableEnemies.Count != 0)
        {
            Creature target = base.Owner.Player.RunState.Rng.CombatTargets.NextItem(hittableEnemies);
            if (target == null)
            {
                return;
            }
            Flash();
            await CreatureCmd.Damage(choiceContext, target, base.Amount, ValueProp.Unpowered, base.Owner, null);
            await PowerCmd.Apply<FrailPower>(choiceContext, target, base.Amount, base.Owner, null);
        }        
    }
}

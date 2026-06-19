using System;
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
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Buff;

public sealed class BloodlettingRitePower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		GetExtraHoverTip()
	];

    public const decimal HpLossPercent = 5;

    private decimal HpLossAmount
    {
        get
        {
            return Math.Floor(base.Owner?.CurrentHp * (HpLossPercent / 100) ?? 0);
        }
    }

	protected override HoverTip GetExtraHoverTip()
	{
		StringBuilder stringBuilder = new();
		LocString locString = ExtraDescription;
        locString.Add("Amount", base.Amount);
        locString.Add("HpLossPercent", HpLossPercent);
        locString.Add("HpLossAmount", HpLossAmount);
		DynamicVars.AddTo(locString);
		stringBuilder.Append(locString.GetFormattedText());
		return new HoverTip(this, stringBuilder.ToString(), true);
	}

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (base.Owner.Player != null && side == base.Owner.Side)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, HpLossAmount, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, base.Owner, null);
            await PlayerCmd.GainEnergy(base.Amount, base.Owner.Player);
        }
    }
}

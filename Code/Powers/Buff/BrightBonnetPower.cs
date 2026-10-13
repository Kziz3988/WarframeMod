using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using WarframeMod.Code.Cards;
using WarframeMod.Code.Cards.Token;

namespace WarframeMod.Code.Powers.Buff;

public sealed class BrightBonnetPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromCard<Sprodling>(),
        HoverTipFactory.ForEnergy(this)
	];

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (base.Owner.Player != null && participants.Contains(base.Owner))
        {
            await WarframeModCard.CreateInHand<Sprodling>(base.Owner.Player, base.Amount, combatState);
        }
    }
}

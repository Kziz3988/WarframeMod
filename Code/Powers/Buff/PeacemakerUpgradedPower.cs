using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using WarframeMod.Code.Cards;
using WarframeMod.Code.Cards.Token;

namespace WarframeMod.Code.Powers.Buff;

public sealed class PeacemakerUpgradedPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromCard<Regulators>(true)
	];

    public override bool ShouldDraw(Player player, bool fromHandDraw)
	{
		if (fromHandDraw)
		{
			return true;
		}
		if (player != base.Owner.Player)
		{
			return true;
		}
		if (player.Creature.Side != player.Creature.CombatState.CurrentSide)
		{
			return true;
		}
		return false;
	}

	public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (base.Owner.Player != null && side == base.Owner.Side)
        {
            var cards = await WarframeModCard.CreateInHand<Regulators>(base.Owner.Player, base.Amount, combatState);
            foreach (Regulators card in cards)
            {
                CardCmd.Upgrade(card, CardPreviewStyle.None);
            }
        }        
    }
}

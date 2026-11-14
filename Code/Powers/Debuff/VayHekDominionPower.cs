
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace WarframeMod.Code.Powers.Debuff;

public partial class VayHekDominionPower : WarframeModPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy(this)
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player == base.Owner.Player)
		{
			PlayerCombatState? playerCombatState = player.PlayerCombatState;
			if (playerCombatState != null && playerCombatState.TurnNumber >= 2)
			{
				await PlayerCmd.LoseEnergy(1m, player);
                await PowerCmd.Remove(this);
			}
		}
	}
}
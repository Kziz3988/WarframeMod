using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace WarframeMod.Code.Powers.Debuff;

public partial class EnergyVampirePower : WarframeModPower
{
	public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented)
		{
			return;
		}
		if (creature != base.Owner)
        {
            return;
        }
        IEnumerable<Creature> players = from c in base.CombatState.GetTeammatesOf(base.Applier)
			where c != null && c.IsAlive && c.IsPlayer
			select c;
		foreach (Creature player in players)
		{
			await PlayerCmd.GainEnergy(base.Amount, player.Player);
		}
    }
}

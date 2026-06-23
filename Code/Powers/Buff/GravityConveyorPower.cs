using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using WarframeMod.Code.Cards.Status;
using WarframeMod.Code.Monsters;

namespace WarframeMod.Code.Powers.Buff;

public sealed class GravityConveyorPower : WarframeModPower
{
    private class Data
	{
		public bool isReviving;
	}

	public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.None;

    private bool IsReviving => GetInternalData<Data>().isReviving;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromCard<ExplosivePowerCell>()
    ];

    protected override object InitInternalData()
	{
		return new Data();
	}

    public void DoRevive()
	{
		GetInternalData<Data>().isReviving = false;
	}

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
	{
		if (!wasRemovalPrevented && creature == base.Owner && creature.Monster is Raptor raptor)
		{
			GetInternalData<Data>().isReviving = true;
			await raptor.TriggerDeadState();
            foreach (Creature player in raptor.CombatState.PlayerCreatures)
            {
                await CardPileCmd.AddToCombatAndPreview<ExplosivePowerCell>(player, PileType.Discard, 1, null);
            }
		}
	}

    public override bool ShouldAllowHitting(Creature creature)
	{
		if (creature != base.Owner)
		{
			return true;
		}
		return !IsReviving;
	}

    public override bool ShouldStopCombatFromEnding()
	{
		return true;
	}

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
	{
		if (creature != base.Owner)
		{
			return true;
		}
		return creature.CombatState?.Players.Any(p => PileType.Hand.GetPile(p).Cards.Where(c => c.GetType() == typeof(ExplosivePowerCell)).Count() > 0) ?? true;
	}

    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
	{
		return false;
	}
}

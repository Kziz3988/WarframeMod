using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace WarframeMod.Code.Powers.Buff;

public sealed class RadialDisarmPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        StunIntent.GetStaticHoverTip(),
        HoverTipFactory.FromPower<WeakPower>()
    ];

    private static bool Stunned(MonsterModel? monster)
	{
        if (monster == null)
        {
            return false;
        }
		return monster.NextMove.Intents.Any(delegate(AbstractIntent intent)
		{
			IntentType intentType = intent.IntentType;
			return intentType == IntentType.Stun;
		});
	}

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (base.Owner.Player != null && side == base.Owner.Side)
        {
            foreach (Creature creature in base.CombatState.HittableEnemies)
            {
                if (Stunned(creature.Monster))
                {
                    await PowerCmd.Apply<WeakPower>(choiceContext, creature, base.Amount, base.Owner, null);
                }
            }
        }
    }
}

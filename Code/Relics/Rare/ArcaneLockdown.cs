using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Relics.Rare;

[Pool(typeof(MagRelicPool))]
public sealed class ArcaneLockdown : WarframeModRelic
{
    private int _currentTurn = 1;

    [SavedProperty]
    private int CurrentTurn
    {
        get
        {
            return _currentTurn;
        }
        set
        {
            AssertMutable();
            _currentTurn = value;
            base.DynamicVars["Turns"].BaseValue = _currentTurn;
            InvokeDisplayAmountChanged();
        }
    }

	public override RelicRarity Rarity => RelicRarity.Rare;
    public override bool ShowCounter => true;
    public override int DisplayAmount => CurrentTurn;

	protected override IEnumerable<IHoverTip> ExtraHoverTips =>[
        StunIntent.GetStaticHoverTip()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Turns", CurrentTurn),
        new DynamicVar("Increment", 1m)
    ];

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
	{
		if (participants.Contains(base.Owner.Creature) && base.Owner.PlayerCombatState?.TurnNumber == CurrentTurn)
		{
			Flash();
            Creature? target = base.Owner.RunState.Rng.CombatTargets.NextItem(base.Owner.Creature.CombatState.HittableEnemies);
			if (target != null)
            {
                await CreatureCmd.Stun(target);
            }
            CurrentTurn++;
            base.Status = RelicStatus.Disabled;
		}
	}

    public override Task AfterCombatEnd(CombatRoom _)
	{
		base.Status = RelicStatus.Normal;
        return Task.CompletedTask;
	}
}

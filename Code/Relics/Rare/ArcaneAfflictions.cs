using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using WarframeMod.Code.Character;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Relics.Rare;

[Pool(typeof(ExcaliburRelicPool))]
public class ArcaneAfflictions : WarframeModRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        StunIntent.GetStaticHoverTip()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Increment", 1m)
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
        if (base.Owner != null && participants.Contains(base.Owner.Creature))
        {
            foreach (Creature creature in base.Owner.Creature.CombatState.HittableEnemies)
            {
                if (!Stunned(creature.Monster))
                {
                    continue;
                }
                Flash();
                var powersToDecrement = creature.Powers
                    .Where(p => p.StackType == PowerStackType.Counter && p.IsTheMoreTheBetter() && ((p.Type == PowerType.Debuff && p.Amount > 0) || (p.Type == PowerType.Buff && p.Amount < 0)))
                    .ToList();
                var powersToIncrement = creature.Powers
                    .Where(p => p.StackType == PowerStackType.Counter && p.IsTheLessTheBetter() && ((p.Type == PowerType.Debuff && p.Amount > 0) || (p.Type == PowerType.Buff && p.Amount < 0)))
                    .ToList();

                foreach (PowerModel power in powersToDecrement)
                {
                    await PowerCmd.ModifyAmount(choiceContext, power, -base.DynamicVars["Increment"].BaseValue, creature, null);
                }
                foreach (PowerModel power in powersToIncrement)
                {
                    await PowerCmd.ModifyAmount(choiceContext, power, base.DynamicVars["Increment"].BaseValue, creature, null);
                }
            }
        }
    }
}
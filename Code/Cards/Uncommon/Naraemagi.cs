using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using WarframeMod.Code.Powers.Buff;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Uncommon;

public class Naraemagi() : WarframeModCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ColdPower>(),
        StunIntent.GetStaticHoverTip(),
        HoverTipFactory.FromPower<ShieldPower>()
    ];

	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0),
        new CalculationExtraVar(1m),
        new CalculatedVar("TotalShield").WithMultiplier((CardModel card, Creature? _) => CountColdStacks(card.Owner.Creature))
    ];

    private static int CountColdStacks(Creature? creature)
    {
        if (creature == null || creature.CombatState == null)
        {
            return 0;
        }
        int count = 0;
        foreach (Creature enemy in creature.CombatState.HittableEnemies)
        {
            ColdPower? coldPower = enemy.GetPower<ColdPower>();
            if (coldPower != null)
            {
                count += coldPower.Amount;
            }
        }
        return count;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int shieldAmount = (int)((CalculatedVar)base.DynamicVars["TotalShield"]).Calculate(base.Owner.Creature);
        await ShieldPower.ApplyShield(base.Owner.Creature, shieldAmount, 0, 0, base.Owner.Creature, this);
        foreach (Creature enemy in base.Owner.Creature.CombatState.HittableEnemies)
        {
            ColdPower? coldPower = enemy.GetPower<ColdPower>();
            if (coldPower != null)
            {
                await PowerCmd.Remove(coldPower);
            }
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}

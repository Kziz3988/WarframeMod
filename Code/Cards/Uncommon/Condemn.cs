using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Uncommon;

public class Condemn() : WarframeModCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        StunIntent.GetStaticHoverTip(),
        HoverTipFactory.FromPower<ShieldPower>()
    ];

	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(0),
        new CalculationExtraVar(2m),
        new CalculatedVar("TotalShield").WithMultiplier((CardModel card, Creature? _) => CountStunnedEnemies(card.Owner.Creature))
    ];

    private static int CountStunnedEnemies(Creature? creature)
    {
        if (creature == null || creature.GetPower<ShieldPower>() == null || creature.CombatState == null)
        {
            return 0;
        }
        int count = 0;
        foreach (Creature enemy in creature.CombatState.HittableEnemies)
        {
            if (enemy.Monster != null && enemy.Monster.IntendsTo(IntentType.Stun))
            {
                count++;
            }
        }
        return count;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		ShieldPower? shield = base.Owner.Creature.GetPower<ShieldPower>();
        if (shield == null)
        {
            return;
        }
        int shieldAmount = (int)((CalculatedVar)base.DynamicVars["TotalShield"]).Calculate(base.Owner.Creature);
        await ShieldPower.ApplyShield(base.Owner.Creature, shieldAmount, 0, 0, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.CalculationExtra.UpgradeValueBy(1m);
    }
}

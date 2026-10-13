using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Cards.Uncommon;

public class Bunraku() : WarframeModCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.RandomEnemy)
{
	protected override bool HasEnergyCostX => true;
    protected override HashSet<CardTag> CanonicalTags => [(CardTag)WarframeModCardTag.Element];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [ElementData.GetStaticHoverTip()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Element", 5m),
        new DynamicVar("Threshold", 3m),
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		int num = ResolveEnergyXValue();
        ElementData count = new();
        for (int i = 0; i < num; i++)
        {
            Creature enemy = base.Owner.RunState.Rng.CombatTargets.NextItem(base.CombatState.HittableEnemies);
            if (enemy == null)
			{
				continue;
			}
            ElementData element = ElementData.RandomElement(base.Owner.RunState.Rng.CombatTargets);
            count += element;
            ElementData total = base.DynamicVars["Element"].IntValue * element;
            await total.Apply(choiceContext, enemy, base.Owner.Creature, this);
        }
        foreach (decimal amount in count.Elements.Values)
        {
            if (amount < base.DynamicVars["Threshold"].BaseValue)
            {
                continue;
            }
            for (int i = 0; i < num; i++)
            {
                Creature enemy = base.Owner.RunState.Rng.CombatTargets.NextItem(base.CombatState.HittableEnemies);
                if (enemy == null)
                {
                    continue;
                }
                ElementData element = ElementData.RandomElement(base.Owner.RunState.Rng.CombatTargets);
                ElementData total = base.DynamicVars["Element"].IntValue * element;
                await total.Apply(choiceContext, enemy, base.Owner.Creature, this);
            }
            break;
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Element"].UpgradeValueBy(1m);
    }
}

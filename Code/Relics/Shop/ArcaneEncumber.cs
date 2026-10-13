using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using WarframeMod.Code.Character;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Relics.Shop;

[Pool(typeof(VoltRelicPool))]
public class ArcaneEncumber : WarframeModRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ElementData.GetStaticHoverTip()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Element", 1m)
    ];

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (applier == base.Owner.Creature && cardSource != null && amount > 0m && power.IsElement())
		{
			Flash();
			ElementData element = ElementData.RandomElement(base.Owner.RunState.Rng.CombatTargets);
            ElementData total = base.DynamicVars["Element"].IntValue * element;
            await total.Apply(choiceContext, power.Owner, base.Owner.Creature, null);
		}
    }
}
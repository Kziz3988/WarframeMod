using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using WarframeMod.Code.Character;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Relics.Common;

[Pool(typeof(VoltRelicPool))]
public class ArcaneTrojan : WarframeModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<VoidPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VulnerablePower>(1m)
    ];

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (base.Owner != null && amount > 0m && applier == base.Owner.Creature && power is VoidPower)
		{
			Flash();
			await PowerCmd.Apply<VulnerablePower>(choiceContext, power.Owner, base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, null);
		}
    }
}
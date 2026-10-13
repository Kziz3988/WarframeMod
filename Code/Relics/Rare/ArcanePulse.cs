using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Hooks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Relics.Rare;

[Pool(typeof(ExcaliburRelicPool))]
public class ArcanePulse : WarframeModRelic, IHealAmountModifier
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new HealVar(1m)
    ];

    decimal IHealAmountModifier.ModifyHealAdditive(Creature creature, decimal amount)
    {
        if (creature == base.Owner.Creature)
        {
            return base.DynamicVars.Heal.BaseValue;
        }
        return 0m;
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature == base.Owner.Creature && delta > 0)
        {
            Flash();
        }
        return Task.CompletedTask;
    }
}
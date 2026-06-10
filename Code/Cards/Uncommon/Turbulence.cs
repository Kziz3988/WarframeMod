using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Uncommon;

public class Turbulence() : WarframeModCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<TurbulencePower>(30m),
        new DynamicVar("Decrement", TurbulencePower.Decrement)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TurbulencePower>(base.Owner.Creature, base.DynamicVars["TurbulencePower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["TurbulencePower"].UpgradeValueBy(10m);
    }
}
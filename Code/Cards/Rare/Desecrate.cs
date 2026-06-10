using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Rare;

public class Desecrate() : WarframeModCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [new GoldVar(10)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DesecratePower>(base.Owner.Creature, base.DynamicVars.Gold.BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Gold.UpgradeValueBy(5m);
    }
}
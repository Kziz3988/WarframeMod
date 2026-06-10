using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Uncommon;

public class Spores() : WarframeModCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FrailPower>()];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<SporesPower>(1m),
        new DynamicVar("Increment", SporesPower.Increment)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SporesPower>(base.Owner.Creature, base.DynamicVars["SporesPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["SporesPower"].UpgradeValueBy(1m);
    }
}
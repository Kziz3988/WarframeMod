using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Uncommon;

public class Prowl() : WarframeModCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<InvisiblePower>()];
	protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
		new PowerVar<InvisiblePower>(1m),
        new PowerVar<ProwlPower>(30m)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<InvisiblePower>(base.Owner.Creature, base.DynamicVars["InvisiblePower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<ProwlPower>(base.Owner.Creature, base.DynamicVars["ProwlPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["InvisiblePower"].UpgradeValueBy(1m);
        base.DynamicVars["ProwlPower"].UpgradeValueBy(10m);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Common;

public class SmokeScreen() : WarframeModCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<InvisiblePower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
		new PowerVar<InvisiblePower>(1),
        new PowerVar<SmokeScreenPower>(2)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await PowerCmd.Apply<InvisiblePower>(base.Owner.Creature, base.DynamicVars["InvisiblePower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<SmokeScreenPower>(base.Owner.Creature, base.DynamicVars["SmokeScreenPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["SmokeScreenPower"].UpgradeValueBy(1m);
    }
}
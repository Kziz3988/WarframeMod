using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Extensions;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Rare;

public class ElementalWard() : WarframeModCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ElementData.GetStaticHoverTip(),
        HoverTipFactory.FromPower<ShieldPower>()
    ];
	protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
		new PowerVar<ElementalWardPower>(1m)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ElementalWardPower>(choiceContext, base.Owner.Creature, base.DynamicVars["ElementalWardPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["ElementalWardPower"].UpgradeValueBy(1m);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Cards.Token;

namespace WarframeMod.Code.Cards.Uncommon;

public class Recompense() : WarframeModCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{	
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromCard<RecompenseDagger>()
	];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(3)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreateInHand<RecompenseDagger>(base.Owner, base.DynamicVars.Cards.IntValue, base.CombatState);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}

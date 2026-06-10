using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Cards.Token;

namespace WarframeMod.Code.Cards.Uncommon;

public class SeekingTalons() : WarframeModCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{	
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Talon>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(8)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreateInHand<Talon>(base.Owner, base.DynamicVars.Cards.IntValue, base.CombatState);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(2m);
        RemoveKeyword(CardKeyword.Exhaust);
    }
}

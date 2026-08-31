using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Cards.Basic;

[Pool(typeof(MagCardPool))]
public class GreedyPull() : WarframeModCard(1, CardType.Skill, CardRarity.Ancient, TargetType.AnyEnemy)
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [StunIntent.GetStaticHoverTip()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GoldVar(15)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		await CreatureCmd.Stun(cardPlay.Target);
        await PlayerCmd.GainGold(base.DynamicVars.Gold.BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Gold.UpgradeValueBy(5m);
        AddKeyword(CardKeyword.Retain);
    }
}

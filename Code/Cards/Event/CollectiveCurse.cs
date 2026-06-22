using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using WarframeMod.Code.Character;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Event;

[Pool(typeof(EventCardPool))]
public class CollectiveCurse() : WarframeModCard(0, CardType.Skill, CardRarity.Event, TargetType.AnyEnemy)
{
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<WarframeModCardPool>();
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<LinkagePower>(),
        HoverTipFactory.FromPower<WeakPower>()
    ];
	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<LinkagePower>(3m),
        new PowerVar<WeakPower>(1m)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await PowerCmd.Apply<LinkagePower>(choiceContext, cardPlay.Target, base.DynamicVars["LinkagePower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, base.DynamicVars["WeakPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["LinkagePower"].UpgradeValueBy(1m);
        base.DynamicVars["WeakPower"].UpgradeValueBy(1m);
    }
}
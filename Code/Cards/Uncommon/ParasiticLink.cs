using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Powers.Buff;
using System;

namespace WarframeMod.Code.Cards.Uncommon;

public class ParasiticLink() : WarframeModCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ParasiticLinkPower>(50m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		await PowerCmd.Apply<ParasiticLinkPower>(choiceContext, cardPlay.Target, base.DynamicVars["ParasiticLinkPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<ParasiticLinkPower>(choiceContext, base.Owner.Creature, base.DynamicVars["ParasiticLinkPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
		base.DynamicVars["ParasiticLinkPower"].UpgradeValueBy(25m);
    }
}

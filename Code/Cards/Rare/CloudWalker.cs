using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Rare;

public class CloudWalker() : WarframeModCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromPower<IntangiblePower>()
	];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
		new PowerVar<CloudWalkerPower>(2)
	];
	public override IEnumerable<CardKeyword> CanonicalKeywords => [
		CardKeyword.Exhaust
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await PowerCmd.Apply<CloudWalkerPower>(choiceContext, base.Owner.Creature, base.DynamicVars["CloudWalkerPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["CloudWalkerPower"].UpgradeValueBy(-1m);
    }
}
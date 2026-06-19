using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Powers.Buff;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Cards.Rare;

public class Reaper() : WarframeModCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<DeathsHarvestPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
		new PowerVar<ReaperPower>(10m)
	];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ReaperPower>(choiceContext, base.Owner.Creature, base.DynamicVars["ReaperPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["ReaperPower"].UpgradeValueBy(5m);
    }
}
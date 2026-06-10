using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Uncommon;

public class NullStar() : WarframeModCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NullStarPower>()];
	protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<NullStarPower>(20m)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<NullStarPower>(base.Owner.Creature, base.DynamicVars["NullStarPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["NullStarPower"].UpgradeValueBy(10m);
    }
}
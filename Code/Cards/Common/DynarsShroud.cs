using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Common;

public class DynarsShroud() : WarframeModCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<InvisiblePower>(),
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1),
        new PowerVar<InvisiblePower>(1),
		new PowerVar<StrengthPower>(1)
	];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        await PowerCmd.Apply<InvisiblePower>(choiceContext, base.Owner.Creature, base.DynamicVars["InvisiblePower"].BaseValue, base.Owner.Creature, this);
		await PowerCmd.Apply<DynarsShroudPower>(choiceContext, base.Owner.Creature, base.DynamicVars.Strength.BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["InvisiblePower"].UpgradeValueBy(1m);
        base.DynamicVars.Strength.UpgradeValueBy(1m);
    }
}
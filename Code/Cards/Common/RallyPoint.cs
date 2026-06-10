using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Common;

public class RallyPoint() : WarframeModCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<ShieldPower>()
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
		new EnergyVar(2),
        new DynamicVar("TotalShield", 1m)
	];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
		await ShieldPower.ApplyShield(base.Owner.Creature, base.DynamicVars["TotalShield"].IntValue, 0, 0, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.RemoveKeyword(CardKeyword.Exhaust);
    }
}
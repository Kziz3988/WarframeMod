using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Uncommon;

public class Metronome() : WarframeModCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromPower<InvisiblePower>(),
		HoverTipFactory.FromPower<StrengthPower>()
	];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
		new PowerVar<InvisiblePower>(1),
		new PowerVar<MetronomePower>(1)
	];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
		//CardKeyword.Ethereal
	];

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await PowerCmd.Apply<InvisiblePower>(choiceContext, base.Owner.Creature, base.DynamicVars["InvisiblePower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<MetronomePower>(choiceContext, base.Owner.Creature, base.DynamicVars["MetronomePower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
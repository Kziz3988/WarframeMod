using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Cards.Common;

public class Pillager() : WarframeModCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Block),
		HoverTipFactory.FromPower<ShieldPower>()
	];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("BlockAmount", 3m),
        new DynamicVar("TotalShield", 1m)
	];

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        int blockAmount = cardPlay.Target.Block;
		await CreatureCmd.LoseBlock(cardPlay.Target, blockAmount);
        int TotalShield = blockAmount / base.DynamicVars["BlockAmount"].IntValue * base.DynamicVars["TotalShield"].IntValue;
        await ShieldPower.ApplyShield(base.Owner.Creature, TotalShield, 0, 0, base.Owner.Creature, this);
	}

    protected override void OnUpgrade()
    {
        base.DynamicVars["BlockAmount"].UpgradeValueBy(-1);
    }
}
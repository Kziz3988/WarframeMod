using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Debuff;

public partial class RubblePower : WarframeModPower
{
	public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.Static(StaticHoverTip.Block)
	];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
		new StringVar("ApplierName")
	];

	public override Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		Creature applier2 = base.Applier;
		if (applier2 != null && applier2.IsMonster)
		{
			((StringVar)base.DynamicVars["ApplierName"]).StringValue = base.Applier.Monster.Title.GetFormattedText();
		}
		return Task.CompletedTask;
	}

	public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
	{
		if (creature != base.Owner)
		{
            return;
		}
        if (base.Applier != null)
        {
            Flash();
		    await CreatureCmd.GainBlock(base.Applier, new BlockVar(base.Amount, ValueProp.Unpowered), null);
        }
        await PowerCmd.Remove(this);
	}
}

using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Relics.Common;

[Pool(typeof(ExcaliburRelicPool))]
public class ArcanePhantasm : WarframeModRelic
{
	private bool _blockGainedThisCombat;

    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2)
    ];

	private bool BlockGainedThisCombat
	{
		get
		{
			return _blockGainedThisCombat;
		}
		set
		{
			AssertMutable();
			_blockGainedThisCombat = value;
		}
	}

	public override Task BeforeCombatStart()
	{
		BlockGainedThisCombat = false;
		base.Status = RelicStatus.Active;
		return Task.CompletedTask;
	}

    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        if (BlockGainedThisCombat || cardSource?.Owner != base.Owner)
		{
			return;
		}
		BlockGainedThisCombat = true;
        Flash();
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), base.DynamicVars.Cards.BaseValue, base.Owner);
        base.Status = RelicStatus.Disabled;
    }

	public override Task AfterCombatEnd(CombatRoom room)
	{
		BlockGainedThisCombat = false;
		base.Status = RelicStatus.Normal;
		return Task.CompletedTask;
	}
}
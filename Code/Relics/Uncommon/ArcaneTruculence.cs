using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Relics.Uncommon;

[Pool(typeof(VoltRelicPool))]
public sealed class ArcaneTruculence : WarframeModRelic
{
	private bool _isActivating;

	private int _blockGained;

	public override RelicRarity Rarity => RelicRarity.Uncommon;

	public override bool ShowCounter => true;

	public override int DisplayAmount
	{
		get
		{
			if (!IsActivating)
			{
				return BlockGained % base.DynamicVars["BlockAmount"].IntValue;
			}
			return base.DynamicVars["BlockAmount"].IntValue;
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DynamicVar("BlockAmount", 15m),
		new PowerVar<VulnerablePower>(1m)
    ];

	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

	private bool IsActivating
	{
		get
		{
			return _isActivating;
		}
		set
		{
			AssertMutable();
			_isActivating = value;
			UpdateDisplay();
		}
	}

	[SavedProperty]
	public int BlockGained
	{
		get
		{
			return _blockGained;
		}
		set
		{
			AssertMutable();
			_blockGained = value;
			UpdateDisplay();
		}
	}

	private void UpdateDisplay()
	{
		if (IsActivating)
		{
			base.Status = RelicStatus.Normal;
		}
		else
		{
			int intValue = base.DynamicVars["BlockAmount"].IntValue;
			base.Status = (BlockGained % intValue == intValue - 1) ? RelicStatus.Active : RelicStatus.Normal;
		}
		InvokeDisplayAmountChanged();
	}

    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        if (creature == base.Owner.Creature && amount > 0)
        {
            int intValue = base.DynamicVars["BlockAmount"].IntValue;
            if (CombatManager.Instance.IsInProgress && BlockGained / intValue < (BlockGained + (int)amount) / intValue)
			{
				TaskHelper.RunSafely(DoActivateVisuals());
                decimal vulnerableAmount = (BlockGained + (int)amount) / intValue * base.DynamicVars.Vulnerable.BaseValue;
                Creature? target = base.Owner.RunState.Rng.CombatTargets.NextItem(base.Owner.Creature.CombatState.HittableEnemies);
				if (target != null)
                {
                    await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), target, vulnerableAmount, base.Owner.Creature, null);
                }
			}
            BlockGained = (BlockGained + (int)amount) % intValue;
        }
    }

	private async Task DoActivateVisuals()
	{
		IsActivating = true;
		Flash();
		await Cmd.Wait(1f);
		IsActivating = false;
	}
}

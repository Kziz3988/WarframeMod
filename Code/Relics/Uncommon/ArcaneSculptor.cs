using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Relics.Uncommon;

[Pool(typeof(ExcaliburRelicPool))]
public sealed class ArcaneSculptor : WarframeModRelic
{
	private bool _isActivating;

	private int _cardsGenerated;

	public override RelicRarity Rarity => RelicRarity.Uncommon;

	public override bool ShowCounter => true;

	public override int DisplayAmount
	{
		get
		{
			if (!IsActivating)
			{
				return CardsGenerated % base.DynamicVars.Cards.IntValue;
			}
			return base.DynamicVars.Cards.IntValue;
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new CardsVar(5),
		new EnergyVar(1)
    ];

	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy(this)
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
	public int CardsGenerated
	{
		get
		{
			return _cardsGenerated;
		}
		set
		{
			AssertMutable();
			_cardsGenerated = value;
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
			int intValue = base.DynamicVars.Cards.IntValue;
			base.Status = (CardsGenerated % intValue == intValue - 1) ? RelicStatus.Active : RelicStatus.Normal;
		}
		InvokeDisplayAmountChanged();
	}

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (card.Owner == base.Owner)
		{
			CardsGenerated++;
			int intValue = base.DynamicVars.Cards.IntValue;
			if (CombatManager.Instance.IsInProgress && CardsGenerated % intValue == 0)
			{
				TaskHelper.RunSafely(DoActivateVisuals());
				await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
			}
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

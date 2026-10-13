using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using WarframeMod.Code.Cards;
using WarframeMod.Code.Character;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Relics.Uncommon;

[Pool(typeof(VoltRelicPool))]
public sealed class ArcaneImpetus : WarframeModRelic
{
	private bool _isActivating;

	private int _attacksPlayed;

	public override RelicRarity Rarity => RelicRarity.Uncommon;

	public override bool ShowCounter => true;

	public override int DisplayAmount
	{
		get
		{
			if (!IsActivating)
			{
				return AttacksPlayed % base.DynamicVars.Cards.IntValue;
			}
			return base.DynamicVars.Cards.IntValue;
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new CardsVar(8),
		new EnergyVar(1)
    ];

	protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        ElementData.GetStaticHoverTip(),
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
	public int AttacksPlayed
	{
		get
		{
			return _attacksPlayed;
		}
		set
		{
			AssertMutable();
			_attacksPlayed = value;
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
			base.Status = (AttacksPlayed % intValue == intValue - 1) ? RelicStatus.Active : RelicStatus.Normal;
		}
		InvokeDisplayAmountChanged();
	}

	public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner == base.Owner && (cardPlay.Card.Tags.Contains((CardTag)WarframeModCardTag.Element) || cardPlay.Card.GetElements().Amount > 0))
		{
			AttacksPlayed++;
			int intValue = base.DynamicVars.Cards.IntValue;
			if (CombatManager.Instance.IsInProgress && AttacksPlayed % intValue == 0)
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

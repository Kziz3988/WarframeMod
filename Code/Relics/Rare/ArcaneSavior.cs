using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using WarframeMod.Code.Character;

namespace WarframeMod.Code.Relics.Rare;

[Pool(typeof(ExcaliburRelicPool))]
public class ArcaneSavior : WarframeModRelic
{
    private bool _wasUsed;

	public override RelicRarity Rarity => RelicRarity.Rare;

	public override bool IsUsedUp => _wasUsed;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<BufferPower>()
    ];

	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new HealVar(1m),
        new PowerVar<BufferPower>(1m)
    ];

	[SavedProperty]
	public bool WasUsed
	{
		get
		{
			return _wasUsed;
		}
		set
		{
			AssertMutable();
			_wasUsed = value;
			if (IsUsedUp)
			{
				base.Status = RelicStatus.Disabled;
			}
		}
	}

	public override bool ShouldDieLate(Creature creature)
	{
		if (creature != base.Owner.Creature)
		{
			return true;
		}
		if (WasUsed)
		{
			return true;
		}
		return false;
	}

	public override async Task AfterPreventingDeath(Creature creature)
	{
		Flash();
		WasUsed = true;
		await CreatureCmd.Heal(creature, base.DynamicVars.Heal.BaseValue);
        await PowerCmd.Apply<BufferPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars["BufferPower"].BaseValue, base.Owner.Creature, null);
	}
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using WarframeMod.Code.Character;
using WarframeMod.Code.Powers.Buff;

namespace WarframeMod.Code.Relics.Rare;

[Pool(typeof(MagRelicPool))]
public sealed class ArcaneBattery : WarframeModRelic
{
	public override RelicRarity Rarity => RelicRarity.Rare;
	protected override IEnumerable<IHoverTip> ExtraHoverTips =>[
        HoverTipFactory.FromPower<ShieldPower>(),
        HoverTipFactory.ForEnergy(this)
    ];

    private ShieldPower? shield = null;

	protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("TotalShield", 10m),
        new EnergyVar(1)
    ];

	private void Initialize(ShieldPower? shieldPower)
    {
        if (shieldPower == null)
        {
            return;
        }
        shield = shieldPower; 
	}

	public override Task AfterObtained()
    {
        ShieldPower? shieldPower = base.Owner.Creature.GetPower<ShieldPower>();
        Initialize(shieldPower);
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnStartLate(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner.Creature) && shield != null)
        {
            int modifier = shield.TotalShield / base.DynamicVars["TotalShield"].IntValue;
            if (modifier > 0)
            {
                Flash();
                await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue * modifier, base.Owner);
            }
        }
    }
}

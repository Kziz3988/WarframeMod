using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Debuff;

public partial class LinkagePower : WarframeModPower
{
	public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner || dealer == null)
        {
            return;
        }
        if (target.GetPower<LinkagePower>() == null)
        {
            return;
        }
        await CreatureCmd.Damage(choiceContext, base.Owner, result.UnblockedDamage, ValueProp.Unpowered | ValueProp.SkipHurtAnim, null, null);
    }

	public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
        if (side != base.Owner.Side)
		{
			return;
		}
		if (base.Owner.IsAlive)
		{
			await PowerCmd.Decrement(this);
		}
		else
		{
			await PowerCmd.Remove(this);
		}
	}
}

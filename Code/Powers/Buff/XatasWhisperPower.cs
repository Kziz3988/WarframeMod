using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using WarframeMod.Code.Powers.Debuff;

namespace WarframeMod.Code.Powers.Buff;

public sealed class XatasWhisperPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
	{
		if (dealer != null && (dealer == base.Owner || dealer.PetOwner?.Creature == base.Owner) && cardSource != null && cardSource.Type == CardType.Attack && props.IsPoweredAttack() && result.TotalDamage > 0)
		{
			await PowerCmd.Apply<VoidPower>(target, base.Amount, base.Owner, null);
		}
	}
}

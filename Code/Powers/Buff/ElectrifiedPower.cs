using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace WarframeMod.Code.Powers.Debuff;

public sealed class ElectrifiedPower : WarframeModPower
{
	public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
		HoverTipFactory.FromPower<ElectricityPower>(),
		StunIntent.GetStaticHoverTip()
	];

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
		if (command.Attacker != base.Owner || command.TargetSide == base.Owner.Side || !command.DamageProps.IsPoweredAttack())
		{
			return;
		}
		List<DamageResult> list = command.Results.SelectMany((List<DamageResult> r) => r).ToList();
		if (!list.Any((DamageResult r) => r.UnblockedDamage > 0))
		{
			return;
		}
		Dictionary<Creature, List<DamageResult>> damageResultsByCreature = new Dictionary<Creature, List<DamageResult>>();
		foreach (DamageResult item in list)
		{
			if (item.Receiver.IsPlayer)
			{
				if (!damageResultsByCreature.ContainsKey(item.Receiver))
				{
					damageResultsByCreature.Add(item.Receiver, new List<DamageResult>());
				}
				damageResultsByCreature[item.Receiver].Add(item);
			}
		}
		bool flag = false;
		foreach (Creature target in damageResultsByCreature.Keys)
		{
			int num = damageResultsByCreature[target].Count((DamageResult r) => r.UnblockedDamage > 0);
			flag = flag || num > 0;
			await PowerCmd.Apply<ElectricityPower>(new ThrowingPlayerChoiceContext(), target, num, base.Owner, null);
		}
		if (flag)
		{
			Flash();
		}
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
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
			await Cmd.CustomScaledWait(0.1f, 0.25f);
		}
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using WarframeMod.Code.Extensions;

namespace WarframeMod.Code.Monsters;

public sealed class PropagandaDrone : CustomMonsterModel
{
	public override string? CustomVisualPath => "propaganda_drone.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 8, 7);

	public override int MaxInitialHp => MinInitialHp;

	private int BuffStrengthGain => 2;

    private int EnergyLose => -1;

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Armor;

	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
		await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
	}

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState moveState = new MoveState("PROPAGANDA_MOVE", PropagandaMove, new BuffIntent(), new DebuffIntent());
		moveState.FollowUpState = moveState;
        list.Add(moveState);
		return new MonsterMoveStateMachine(list, moveState);
	}

	private async Task PropagandaMove(IReadOnlyList<Creature> targets)
	{
		await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), base.CombatState.HittableEnemies, BuffStrengthGain, base.Creature, null);
        await PowerCmd.Apply<EnergyNextTurnPower>(new ThrowingPlayerChoiceContext(), targets, EnergyLose, base.Creature, null);
	}
}

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

public sealed class OrbitalStrikeDrone : CustomMonsterModel
{
	public override string? CustomVisualPath => "orbital_strike_drone.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 8, 7);

	public override int MaxInitialHp => MinInitialHp;

	private int OrbitalStrikeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 30, 25);

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Armor;

	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
		await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
	}

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState moveState = new MoveState("ORBITAL_STRIKE_MOVE", OrbitalStrikeMove, new SingleAttackIntent(OrbitalStrikeDamage));
		moveState.FollowUpState = moveState;
        list.Add(moveState);
		return new MonsterMoveStateMachine(list, moveState);
	}

	private async Task OrbitalStrikeMove(IReadOnlyList<Creature> targets)
	{
		await DamageCmd.Attack(OrbitalStrikeDamage).FromMonster(this)
			.Execute(null);
	}
}

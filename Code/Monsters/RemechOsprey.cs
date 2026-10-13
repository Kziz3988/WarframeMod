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

public sealed class RemechOsprey : CustomMonsterModel
{
	public override string? CustomVisualPath => "remech_osprey.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 21, 18);

	public override int MaxInitialHp => MinInitialHp;

	public override DamageSfxType TakeDamageSfxType => DamageSfxType.Armor;

	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
		await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
	}

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState moveState = new MoveState("REMECH_LASER_MOVE", RemechLaserMove, new HealIntent());
		moveState.FollowUpState = moveState;
        list.Add(moveState);
		return new MonsterMoveStateMachine(list, moveState);
	}

	private async Task RemechLaserMove(IReadOnlyList<Creature> targets)
	{
		foreach (Creature monster in base.CombatState.HittableEnemies)
        {
            if (monster.Monster is not RemechOsprey)
            {
                await CreatureCmd.Heal(monster, monster.MaxHp / 3);
            }
        }
	}
}

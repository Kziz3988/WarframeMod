using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using BaseLib.Abstracts;
using WarframeMod.Code.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace WarframeMod.Code.Monsters;

public sealed class Leaper : CustomMonsterModel
{
    public override string? CustomVisualPath => "leaper.tscn".CharacterAnimationScenePath();

	public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 7, 6);

	public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 8, 7);

    private int LeapDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);

    private int LeapRepeat => 2;

    private int DebuffWeakApply => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 2, 1);
    
    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
		await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);
    }

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState leapMove = new MoveState("LEAP_MOVE", LeapMove, new MultiAttackIntent(LeapDamage, LeapRepeat), new DebuffIntent());
		leapMove.FollowUpState = leapMove;
		list.Add(leapMove);
		return new MonsterMoveStateMachine(list, leapMove);
	}

	private async Task LeapMove(IReadOnlyList<Creature> targets)
	{
        await DamageCmd.Attack(LeapDamage).WithHitCount(LeapRepeat).FromMonster(this)
			.Execute(null);
        await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, DebuffWeakApply, base.Creature, null);
	}
}
